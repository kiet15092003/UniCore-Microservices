using StudentService.Business.Services;

namespace StudentService.Business
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IStudentSvc, StudentSvc>();
        }
    }
}
