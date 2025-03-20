using CourseService.CommunicationTypes.KafkaService.KafkaConsumer.Services;

namespace CourseService.CommunicationTypes
{
    public static class CommunicationTypesRegistration
    {
        public static void AddCommunicationTypes(this IServiceCollection services)
        {
            services.AddScoped<IKafkaConsumerFunction, KafkaConsumerFunction>();
        }
    }
}
