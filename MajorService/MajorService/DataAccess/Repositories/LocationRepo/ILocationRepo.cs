using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.DataAccess.Repositories.LocationRepo
{
    public interface ILocationRepo
    {
        Task<List<Location>> GetAllLocationsAsync();
        Task<Location> GetLocationByIdAsync(Guid id);
        Task<Location> CreateLocationAsync(Location location);
        Task<bool> DeactivateLocationAsync(Guid id);
        Task<bool> ActivateLocationAsync(Guid id);
        Task<PaginationResult<Location>> GetLocationsByPaginationAsync(
            Pagination pagination,
            LocationListFilterParams filterParams,
            Order? order);
        Task<int> GetTotalBuildingsForLocationAsync(Guid locationId);
        Task<int> GetTotalFloorsForLocationAsync(Guid locationId);
        Task<int> GetTotalRoomForLocationAsync(Guid locationId);
    }
}
