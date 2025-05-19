using System.Collections.Concurrent;
using System.Threading.Channels;
using StudentService.CommunicationTypes.KafkaService.KafkaConsumer.Templates;
using UserService.CommunicationTypes.Http.HttpClient;

namespace NotificationService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly SmtpClientService _smtpClientService;
        private readonly Channel<UserImportedEventDataSingleData> _emailQueue;

        public Worker(ILogger<Worker> logger, SmtpClientService smtpClientService)
        {
            _logger = logger;
            _smtpClientService = smtpClientService;
            // Create unbounded channel for queuing email creation tasks
            _emailQueue = Channel.CreateUnbounded<UserImportedEventDataSingleData>(new UnboundedChannelOptions 
            { 
                SingleReader = true, 
                SingleWriter = false 
            });
        }

        // Method to queue email creation tasks
        public async Task QueueEmailCreationAsync(UserImportedEventData userData)
        {
            if (userData?.Users == null || !userData.Users.Any())
            {
                _logger.LogWarning("No users provided to create emails for");
                return;
            }

            _logger.LogInformation($"Queuing {userData.Users.Count} email creation tasks");
            
            foreach (var user in userData.Users)
            {
                if (!string.IsNullOrEmpty(user.UserEmail) && !string.IsNullOrEmpty(user.Password))
                {
                    await _emailQueue.Writer.WriteAsync(user);
                }
                else
                {
                    _logger.LogWarning($"Skipping invalid user data for email: {user.UserEmail}");
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email creation worker started at: {time}", DateTimeOffset.Now);
            
            // Process emails in the background
            await ProcessEmailQueueAsync(stoppingToken);
        }

        private async Task ProcessEmailQueueAsync(CancellationToken stoppingToken)
        {
            try
            {
                // Continue processing until cancellation is requested
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        // Wait for the next email creation task with cancellation support
                        var user = await _emailQueue.Reader.ReadAsync(stoppingToken);
                        
                        // Process the email creation task
                        await CreateEmailAsync(user);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        // Normal cancellation, break the loop
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing email from queue");
                        // Continue processing other emails even if one fails
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error in email processing worker");
            }
            finally
            {
                _logger.LogInformation("Email creation worker shutting down at: {time}", DateTimeOffset.Now);
            }
        }

        private async Task CreateEmailAsync(UserImportedEventDataSingleData user)
        {
            try
            {
                _logger.LogInformation($"Creating email account for: {user.UserEmail}");
                
                var result = await _smtpClientService.CreateEmailAccountAsync(user.UserEmail, user.Password);
                
                if (result)
                {
                    _logger.LogInformation($"Successfully created email account for: {user.UserEmail}");
                }
                else
                {
                    _logger.LogError($"Failed to create email account for: {user.UserEmail}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating email account for: {user.UserEmail}");
            }
        }
    }
}
