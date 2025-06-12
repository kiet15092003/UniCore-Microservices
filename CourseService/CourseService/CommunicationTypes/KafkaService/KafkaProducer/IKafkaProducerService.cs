using System.Threading.Tasks;

namespace CourseService.CommunicationTypes.KafkaService.KafkaProducer
{
    public interface IKafkaProducerService
    {
        Task PublishMessageAsync<T>(string topic, T message);
    }
}
