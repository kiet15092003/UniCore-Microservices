using System.Threading.Tasks;

namespace UserService.CommunicationTypes.KafkaService.KafkaProducer
{
    public interface IKafkaProducerService
    {
        Task PublishMessageAsync<T>(string topic, T message);
    }
}
