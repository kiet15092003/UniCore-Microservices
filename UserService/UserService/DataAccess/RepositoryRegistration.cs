using UserService.DataAccess.Repositories.AuthRepo;
using UserService.DataAccess.Repositories.TrainingManagerRepo;
using UserService.DataAccess.Repositories.UserRepo;

namespace UserService.DataAccess
{
    public static class RepositoryRegistration
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepo, AuthRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<ITrainingManagerRepo, TrainingManagerRepo>();
        }
    }
}
