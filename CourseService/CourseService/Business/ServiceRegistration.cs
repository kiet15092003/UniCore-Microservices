using CourseService.Business.Services;
using CourseService.Utils;

namespace CourseService.Business
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {            
            services.AddScoped<ICourseService, CourseSvc>();
            services.AddScoped<ITrainingRoadmapService, TrainingRoadmapSvc>();
            services.AddScoped<ICoursesGroupService, CoursesGroupSvc>();
            services.AddScoped<ISemesterService, SemesterService>();
            services.AddScoped<ICourseMaterialService, CourseMaterialService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IShiftService, ShiftService>();
            services.AddScoped<IAcademicClassService, AcademicClassService>();
        }
    }
}
