using CourseService.DataAccess.Repositories;

namespace CourseService.DataAccess
{
    public static class RepositoryRegistration
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ITrainingRoadmapRepository, TrainingRoadmapRepository>();
            services.AddScoped<ICoursesGroupRepository, CoursesGroupRepository>();
        }
    }
}
