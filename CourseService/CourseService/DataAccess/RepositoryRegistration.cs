using CourseService.DataAccess.Repositories.MajorRepo;
using CourseService.DataAccess.Repositories.StudentRepo;
using CourseService.DataAccess.Repositories.TrainingManagerRepo;

namespace CourseService.DataAccess
{
    public static class RepositoryRegistration
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IStudentRepo, StudentRepo>();
            services.AddScoped<ITrainingManagerRepo, TrainingManagerRepo>();
            services.AddScoped<IMajorRepo, MajorRepo>();
        }
    }
}
