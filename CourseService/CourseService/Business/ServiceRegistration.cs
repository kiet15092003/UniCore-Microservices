using CourseService.Business.Services.StudentService;

namespace CourseService.Business
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IStudentService, StudentService>();
        }
    }
}
