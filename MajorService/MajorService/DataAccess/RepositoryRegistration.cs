using MajorService.DataAccess.Repositories.MajorRepo;

namespace MajorService.DataAccess
{
    public static class RepositoryRegistration
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IMajorRepo, MajorRepo>();
        }
    }
}
