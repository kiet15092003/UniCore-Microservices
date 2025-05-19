using UserService.CommunicationTypes.KafkaService.KafkaConsumer.Templates;
using StudentService.CommunicationTypes.KafkaService.KafkaConsumer.Templates;
using UserService.CommunicationTypes.Http.HttpClient;
using Microsoft.Extensions.Logging;
using NotificationService;

namespace UserService.CommunicationTypes.KafkaService.KafkaConsumer.Services
{
    public class KafkaConsumerFunction : IKafkaConsumerFunction
    {
        private readonly ILogger<KafkaConsumerFunction> _logger;
        private readonly Worker _emailWorker;

        public KafkaConsumerFunction(
            ILogger<KafkaConsumerFunction> logger,
            Worker emailWorker)
        {
            _logger = logger;
            _emailWorker = emailWorker;
        }

        public async Task<bool> CreateEmails(UserImportedEventData userData)
        {
            try
            {
                _logger.LogInformation("Queueing email creation tasks for imported users");
                
                if (userData?.Users == null || !userData.Users.Any())
                {
                    _logger.LogWarning("No users provided in the UserImportedEventData");
                    return false;
                }

                // Queue the email creation tasks to be processed by the Worker background service
                await _emailWorker.QueueEmailCreationAsync(userData);
                
                _logger.LogInformation($"Successfully queued {userData.Users.Count} email creation tasks");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while queueing email creation tasks");
                return false;
            }
        }
    }
}
