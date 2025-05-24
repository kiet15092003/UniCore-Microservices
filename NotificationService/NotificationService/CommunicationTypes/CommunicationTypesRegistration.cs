using UserService.CommunicationTypes.KafkaService.KafkaConsumer.Services;

namespace UserService.CommunicationTypes
{
    public static class CommunicationTypesRegistration
    {
        public static void AddCommunicationTypes(this IServiceCollection services)
        {
           services.AddScoped<IKafkaConsumerFunction, KafkaConsumerFunction>();
        }
    }
}
