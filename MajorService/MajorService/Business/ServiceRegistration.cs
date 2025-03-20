using MajorService.Business.Services.MajorServices;

namespace MajorService.Business
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IMajorSvc, MajorSvc>();
        }
    }
}
