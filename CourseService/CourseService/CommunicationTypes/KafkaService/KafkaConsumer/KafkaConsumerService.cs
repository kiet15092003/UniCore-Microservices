using Confluent.Kafka;
using CourseService.CommunicationTypes.KafkaService.KafkaConsumer.Services;
using CourseService.CommunicationTypes.KafkaService.KafkaConsumer.Templates;
using System.Text.Json;

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

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = Guid.NewGuid().ToString(),
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
            _logger.LogInformation($"✅ Processing message: \"{message}\"");

            var eventData = JsonSerializer.Deserialize<BaseEvent>(message);
            if (eventData == null || eventData.Data == null)
            {
                _logger.LogWarning("⚠️ Invalid message format.");
                return;
            }

            using (var scope = _scopeFactory.CreateScope()) // Create a scoped DI container
            {
                var kafkaConsumerFunction = scope.ServiceProvider.GetRequiredService<IKafkaConsumerFunction>();
                switch (eventData.EventType)
                {
                    case "MajorSeededEvent":
                        var majorSeededData = JsonSerializer.Deserialize<MajorSeededEventData>(eventData.Data.ToString());
                        if (majorSeededData != null)
                            await kafkaConsumerFunction.SeedMajors(majorSeededData);
                        break;

                    case "TrainingManagerCreatedEvent":
                        var trainingManagerCreatedData = JsonSerializer.Deserialize<TrainingManagerCreatedEventData>(eventData.Data.ToString());
                        if (trainingManagerCreatedData != null)
                            await kafkaConsumerFunction.CreateTrainingManager(trainingManagerCreatedData);
                        break;

                    case "StudentCreatedEvent":
                        var studentCreatedData = JsonSerializer.Deserialize<StudentCreatedEventData>(eventData.Data.ToString());
                        if (studentCreatedData != null)
                            await kafkaConsumerFunction.CreateStudent(studentCreatedData);
                        break;

                    default:
                        _logger.LogWarning($"⚠️ Unknown event type \"{eventData.EventType}\", skipping processing.");
                        break;
                }
            }

            _logger.LogInformation($"✅ Successfully processed event: \"{eventData.EventType}\"");
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error processing message: {ex.Message}");
        }
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}
