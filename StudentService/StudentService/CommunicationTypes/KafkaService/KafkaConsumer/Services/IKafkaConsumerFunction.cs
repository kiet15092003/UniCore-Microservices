using StudentService.CommunicationTypes.KafkaService.KafkaConsumer.Templates;
using StudentService.Entities;

namespace StudentService.CommunicationTypes.KafkaService.KafkaConsumer.Services
{
    public interface IKafkaConsumerFunction
    {
        Task<Student> CreateStudent(StudentCreatedEventData data);
    }
}
