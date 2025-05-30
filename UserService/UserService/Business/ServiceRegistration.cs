using UserService.Business.Services.AuthService;
using UserService.Business.Services.BatchService;
using UserService.Business.Services.StudentService;
using UserService.Business.Services.GuardianService;
using UserService.Business.Services.AddressService;
using UserService.Business.Services.LecturerService;
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
            services.AddScoped<IGuardianService, GuardianService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<ILecturerService, LecturerService>();

        }
    }
}
