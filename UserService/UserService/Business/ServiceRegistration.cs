using UserService.Business.Services.AuthService;
using UserService.Business.Services.BatchService;
using UserService.Business.Services.StudentService;
using UserService.CommunicationTypes.Grpc.GrpcClient;

namespace UserService.Business
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBatchService, BatchService>();
            services.AddHttpClient();
            services.AddScoped<IStudentService, StudentService>();
        }
    }
}
