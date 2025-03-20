using StudentService.CommunicationTypes.KafkaService.KafkaConsumer.Services;

namespace StudentService.CommunicationTypes
{
    public static class CommunicationTypesRegistration
    {
        public static void AddCommunicationTypes(this IServiceCollection services)
        {
           services.AddScoped<IKafkaConsumerFunction, KafkaConsumerFunction>();
        }
    }
}
