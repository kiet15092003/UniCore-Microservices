using UserService.DataAccess.Repositories.AuthRepo;
using UserService.DataAccess.Repositories.DepartmentRepo;
using UserService.DataAccess.Repositories.LecturerRepo;
using UserService.DataAccess.Repositories.TrainingManagerRepo;

namespace UserService.DataAccess
{
    public static class RepositoryRegistration
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepo, AuthRepo>();
            services.AddScoped<ILecturerRepo, LecturerRepo>();
            services.AddScoped<IDepartmentRepo, DepartmentRepo>();
            services.AddScoped<ITrainingManagerRepo, TrainingManagerRepo>();
        }
    }
}
