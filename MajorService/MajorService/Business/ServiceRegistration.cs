using MajorService.Business.Services.BuildingServices;
using MajorService.Business.Services.DepartmentServices;
using MajorService.Business.Services.FloorServices;
using MajorService.Business.Services.LocationServices;
using MajorService.Business.Services.MajorGroupServices;
using MajorService.Business.Services.MajorServices;
using MajorService.Business.Services.RoomServices;

namespace MajorService.Business
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {            services.AddScoped<IMajorSvc, MajorSvc>();
            services.AddScoped<IDepartmentSvc, DepartmentSvc>();
            services.AddScoped<IMajorGroupSvc, MajorGroupSvc>();
            services.AddScoped<ILocationSvc, LocationSvc>();
            services.AddScoped<IBuildingSvc, BuildingSvc>();
            services.AddScoped<IFloorSvc, FloorSvc>();
            services.AddScoped<IRoomSvc, RoomSvc>();
        }
    }
}
