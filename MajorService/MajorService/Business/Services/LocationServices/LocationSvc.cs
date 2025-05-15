using AutoMapper;
using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Location;
using MajorService.DataAccess.Repositories.LocationRepo;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.Business.Services.LocationServices
{
    public class LocationSvc : ILocationSvc
    {
        private readonly ILocationRepo _locationRepo;
        private readonly IMapper _mapper;

        public LocationSvc(ILocationRepo locationRepo, IMapper mapper)
        {
            _locationRepo = locationRepo;
            _mapper = mapper;
        }        
        public async Task<List<LocationReadDto>> GetAllLocationsAsync()
        {
            var locations = await _locationRepo.GetAllLocationsAsync();
            var locationDtos = _mapper.Map<List<LocationReadDto>>(locations);
            
            foreach (var locationDto in locationDtos)
            {
                locationDto.TotalBuilding = await _locationRepo.GetTotalBuildingsForLocationAsync(locationDto.Id);
                locationDto.TotalFloor = await _locationRepo.GetTotalFloorsForLocationAsync(locationDto.Id);
                locationDto.TotalRoom = await _locationRepo.GetTotalRoomForLocationAsync(locationDto.Id);
            }
            
            return locationDtos;
        }
        public async Task<LocationReadDto> GetLocationByIdAsync(Guid id)
        {
            var location = await _locationRepo.GetLocationByIdAsync(id);
            var locationDto = _mapper.Map<LocationReadDto>(location);
            
            locationDto.TotalBuilding = await _locationRepo.GetTotalBuildingsForLocationAsync(id);
            locationDto.TotalFloor = await _locationRepo.GetTotalFloorsForLocationAsync(id);
            locationDto.TotalRoom = await _locationRepo.GetTotalRoomForLocationAsync(id);
            
            return locationDto;
        }        
        public async Task<LocationReadDto> CreateNewLocationAsync(CreateNewLocationDto request)
        {
            var location = _mapper.Map<Location>(request);
            var createdLocation = await _locationRepo.CreateLocationAsync(location);
            var locationDto = _mapper.Map<LocationReadDto>(createdLocation);
            
            // For a new location, these will be 0, but include them for consistency
            locationDto.TotalBuilding = 0;
            locationDto.TotalFloor = 0;
            locationDto.TotalRoom = 0;
            
            return locationDto;
        }

        public async Task<bool> DeactivateLocationAsync(DeactivateDto deactivateDto)
        {
            return await _locationRepo.DeactivateLocationAsync(deactivateDto.Id);
        }

        public async Task<bool> ActivateLocationAsync(ActivateDto activateDto)
        {
            return await _locationRepo.ActivateLocationAsync(activateDto.Id);
        }        
        public async Task<LocationListResponse> GetLocationsByPaginationAsync(
            Pagination pagination,
            LocationListFilterParams filterParams,
            Order? order)
        {
            var result = await _locationRepo.GetLocationsByPaginationAsync(pagination, filterParams, order);
            var dtos = _mapper.Map<List<LocationReadDto>>(result.Data);

            foreach (var dto in dtos)
            {
                dto.TotalBuilding = await _locationRepo.GetTotalBuildingsForLocationAsync(dto.Id);
                dto.TotalFloor = await _locationRepo.GetTotalFloorsForLocationAsync(dto.Id);
                dto.TotalRoom = await _locationRepo.GetTotalRoomForLocationAsync(dto.Id);
            }

            return new LocationListResponse
            {
                Data = dtos,
                Total = result.Total,
                PageSize = result.PageSize,
                PageIndex = result.PageIndex
            };
        }
    }
}
