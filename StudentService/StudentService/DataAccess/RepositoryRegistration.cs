using StudentService.DataAccess.Repositories.BatchRepo;
using StudentService.DataAccess.Repositories.StudentRepo;

namespace StudentService.DataAccess
{
    public static class RepositoryRegistration
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IStudentRepo, StudentRepo>();
            services.AddScoped<IBatchRepo, BatchRepo>();    
        }
    }
}
