using EnrollmentService.CommunicationTypes.KafkaService.KafkaConsumer.Templates;

namespace EnrollmentService.CommunicationTypes.KafkaService.KafkaConsumer.Services
{
    public interface IKafkaConsumerFunction
    {
        Task<bool> HandleClassClosureAsync(ClassClosureEventData classClosureData);
    }
}
