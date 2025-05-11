using UserService.DataAccess.Repositories.AuthRepo;
using UserService.DataAccess.Repositories.TrainingManagerRepo;
using UserService.DataAccess.Repositories.UserRepo;
using UserService.DataAccess.Repositories.BatchRepo;
using UserService.DataAccess.Repositories.StudentRepo;
namespace UserService.DataAccess
{
    public static class RepositoryRegistration
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepo, AuthRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<ITrainingManagerRepo, TrainingManagerRepo>();
            services.AddScoped<IBatchRepo, BatchRepo>();
            services.AddScoped<IStudentRepo, StudentRepo>();
        }
    }
}
