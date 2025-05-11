using UserService.Business.Services.AuthService;
using UserService.CommunicationTypes.Grpc.GrpcClient;

namespace UserService.Business
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddHttpClient();
        }
    }
}
