using MajorService.DataAccess.Repositories.BuildingRepo;
using MajorService.DataAccess.Repositories.DepartmentRepo;
using MajorService.DataAccess.Repositories.FloorRepo;
using MajorService.DataAccess.Repositories.LocationRepo;
using MajorService.DataAccess.Repositories.MajorGroupRepo;
using MajorService.DataAccess.Repositories.MajorRepo;
using MajorService.DataAccess.Repositories.RoomRepo;

namespace MajorService.DataAccess
{
    public static class RepositoryRegistration
    {
        public static void AddRepositories(this IServiceCollection services)
        {            services.AddScoped<IMajorRepo, MajorRepo>();
            services.AddScoped<IDepartmentRepo, DepartmentRepo>();
            services.AddScoped<IMajorGroupRepo, MajorGroupRepo>();
            services.AddScoped<ILocationRepo, LocationRepo>();
            services.AddScoped<IBuildingRepo, BuildingRepo>();
            services.AddScoped<IFloorRepo, FloorRepo>();
            services.AddScoped<IRoomRepo, RoomRepo>();
        }
    }
}
