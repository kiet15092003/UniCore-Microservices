using CourseService.Business.Services;

namespace CourseService.Business
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICourseService, CourseSvc>();
        }
    }
}
