using StudentService.Business.Services;
using StudentService.Business.Services.BatchService;

namespace StudentService.Business
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IStudentSvc, StudentSvc>();
            services.AddScoped<IBatchService, BatchService>();
        }
    }
}
