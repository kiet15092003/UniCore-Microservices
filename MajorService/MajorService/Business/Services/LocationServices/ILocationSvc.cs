using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Location;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.Business.Services.LocationServices
{    public interface ILocationSvc
    {
        Task<List<LocationReadDto>> GetAllLocationsAsync();
        Task<LocationReadDto> GetLocationByIdAsync(Guid id);
        Task<LocationReadDto> CreateNewLocationAsync(CreateNewLocationDto request);
        Task<bool> DeactivateLocationAsync(DeactivateDto deactivateDto);
        Task<bool> ActivateLocationAsync(ActivateDto activateDto);
        Task<LocationListResponse> GetLocationsByPaginationAsync(
            Pagination pagination,
            LocationListFilterParams filterParams,
            Order? order);
    }
}
