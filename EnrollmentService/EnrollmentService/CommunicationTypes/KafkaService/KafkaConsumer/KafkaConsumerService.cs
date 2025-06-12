using Confluent.Kafka;
using EnrollmentService.CommunicationTypes.KafkaService.KafkaConsumer.Services;
using EnrollmentService.CommunicationTypes.KafkaService.KafkaConsumer.Templates;
using System.Text.Json;

namespace EnrollmentService.CommunicationTypes.KafkaService.KafkaConsumer
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IConsumer<string, string> _consumer;
        private readonly List<string> _topics;
        private readonly IConfiguration _configuration;

        public KafkaConsumerService(
            IServiceScopeFactory scopeFactory,
            ILogger<KafkaConsumerService> logger,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _configuration = configuration;

            // Get topics from configuration
            _topics = _configuration.GetSection("Kafka:Topics").Get<List<string>>() ?? new List<string>();

            Console.WriteLine(_configuration["Kafka:BootstrapServers"]);
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = _configuration["Kafka:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                SocketTimeoutMs = 10000,
                SessionTimeoutMs = 30000
            };

            _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 Kafka Consumer Service is starting...");

            if (_topics.Count == 0)
            {
                _logger.LogWarning("⚠️ No topics found in configuration! Consumer will not start.");
                return;
            }

            _consumer.Subscribe(_topics);

            await Task.Run(async () =>
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = _consumer.Consume(stoppingToken);

                            if (consumeResult != null)
                            {
                                _logger.LogInformation($"📥 Received message from [{consumeResult.Topic}]: \"{consumeResult.Value}\"");

                                // Process message asynchronously
                                await ProcessMessage(consumeResult.Value);

                                // ✅ Manually commit offset after successful processing
                                _consumer.Commit(consumeResult);
                            }
                        }
                        catch (ConsumeException ex)
                        {
                            _logger.LogError($"❌ Kafka consume error: {ex.Error.Reason}");
                        }
                        catch (OperationCanceledException)
                        {
                            _logger.LogWarning("Kafka Consumer Service is stopping...");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"❌ Unexpected error consuming Kafka message: {ex.Message}");
                        }
                    }
                }
                finally
                {
                    _consumer.Close();
                }
            }, stoppingToken);
        }

        private async Task ProcessMessage(string message)
        {
            try
            {
                _logger.LogInformation("🔄 Processing message: {Message}", message);

                using var scope = _scopeFactory.CreateScope();
                var kafkaConsumerFunction = scope.ServiceProvider.GetRequiredService<IKafkaConsumerFunction>();

                // Try to deserialize as ClassClosureEventData
                var classClosureData = JsonSerializer.Deserialize<ClassClosureEventData>(message);
                if (classClosureData != null && classClosureData.EventType == "ClassClosureEvent")
                {
                    _logger.LogInformation("📋 Processing ClassClosureEvent");
                    await kafkaConsumerFunction.HandleClassClosureAsync(classClosureData);
                    return;
                }

                _logger.LogWarning("⚠️ Unknown message type or unable to deserialize message");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing Kafka message: {Message}", message);
            }
        }

        public override void Dispose()
        {
            _consumer?.Dispose();
            _logger.LogInformation("🛑 Kafka Consumer disposed.");
            base.Dispose();
        }
    }
}
