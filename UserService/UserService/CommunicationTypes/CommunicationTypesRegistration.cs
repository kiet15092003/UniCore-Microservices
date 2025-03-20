using UserService.Business.Services.AuthService;
using UserService.CommunicationTypes.KafkaService.KafkaProducer;

namespace UserService.CommunicationTypes
{
    public static class CommunicationTypesRegistration
    {
        public static void AddCommunicationTypes(this IServiceCollection services)
        {
            services.AddScoped<IKafkaProducerService, KafkaProducerService>();
        }
    }
}
