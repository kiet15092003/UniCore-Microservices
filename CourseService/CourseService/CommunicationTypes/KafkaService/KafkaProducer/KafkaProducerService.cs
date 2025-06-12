using Confluent.Kafka;
using System.Text.Json;

namespace CourseService.CommunicationTypes.KafkaService.KafkaProducer
{
    public class KafkaProducerService : IKafkaProducerService, IDisposable
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;

        public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger)
        {
            var bootstrapServers = configuration["Kafka:BootstrapServers"];
            _logger = logger;

            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                Acks = Acks.All, 
                MessageTimeoutMs = 5000, 
                CompressionType = CompressionType.Lz4 
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
            _logger.LogInformation($"🚀 Kafka Producer initialized with broker: {bootstrapServers}");
        }

        public async Task PublishMessageAsync<T>(string topic, T message)
        {
            try
            {
                var serializedMessage = JsonSerializer.Serialize(message);

                var deliveryResult = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = serializedMessage });

                _logger.LogInformation($"✔️ Kafka Message Sent to [{topic}] (Partition: {deliveryResult.Partition}, Offset: {deliveryResult.Offset}): {serializedMessage}");
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError($"❌ Kafka Message Send Failed: {ex.Error.Reason}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Unexpected Error in KafkaProducerService: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _producer?.Dispose();
            _logger.LogInformation("🛑 Kafka Producer disposed.");
        }
    }
}
