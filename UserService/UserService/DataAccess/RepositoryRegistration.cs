using UserService.DataAccess.Repositories.AuthRepo;
using UserService.DataAccess.Repositories.TrainingManagerRepo;
using UserService.DataAccess.Repositories.UserRepo;
using UserService.DataAccess.Repositories.BatchRepo;
using UserService.DataAccess.Repositories.StudentRepo;
using UserService.DataAccess.Repositories.GuardianRepo;
using UserService.DataAccess.Repositories.AddressRepo;

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
            services.AddScoped<IGuardianRepo, GuardianRepo>();
            services.AddScoped<IAddressRepo, AddressRepo>();
        }
    }
}
