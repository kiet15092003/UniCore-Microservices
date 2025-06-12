using CourseService.CommunicationTypes.KafkaService.KafkaProducer;

namespace CourseService.CommunicationTypes
{
    public static class CommunicationTypesRegistration
    {
        public static void AddCommunicationTypes(this IServiceCollection services)
        {
            services.AddScoped<IKafkaProducerService, KafkaProducerService>();
        }
    }
}
