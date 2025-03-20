using CourseService.CommunicationTypes.KafkaService.KafkaConsumer.Templates;
using CourseService.Entities;

namespace CourseService.CommunicationTypes.KafkaService.KafkaConsumer.Services
{
    public interface IKafkaConsumerFunction
    {
        Task SeedMajors(MajorSeededEventData data);
        Task<TrainingManager> CreateTrainingManager(TrainingManagerCreatedEventData data);
        Task<Student> CreateStudent(StudentCreatedEventData data);
    }
}
