using EnrollmentService.CommunicationTypes.KafkaService.KafkaConsumer;
using EnrollmentService.CommunicationTypes.KafkaService.KafkaConsumer.Services;

namespace EnrollmentService.CommunicationTypes
{
    public static class CommunicationTypesRegistration
    {
        public static void AddCommunicationTypes(this IServiceCollection services)
        {
            services.AddScoped<IKafkaConsumerFunction, KafkaConsumerFunction>();
        }
    }
}
