using UserService.CommunicationTypes.KafkaService.KafkaConsumer.Templates;
using StudentService.CommunicationTypes.KafkaService.KafkaConsumer.Templates;

namespace UserService.CommunicationTypes.KafkaService.KafkaConsumer.Services
{
    public interface IKafkaConsumerFunction
    {
        Task<bool> CreateEmails(UserImportedEventData userData);
    }
}
