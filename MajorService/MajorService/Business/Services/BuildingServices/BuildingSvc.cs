using AutoMapper;
using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Building;
using MajorService.DataAccess.Repositories.BuildingRepo;
using MajorService.DataAccess.Repositories.LocationRepo;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MajorService.Business.Services.BuildingServices
{    public class BuildingSvc : IBuildingSvc
    {
        private readonly IBuildingRepo _buildingRepo;
        private readonly ILocationRepo _locationRepo;
        private readonly IMapper _mapper;

        public BuildingSvc(IBuildingRepo buildingRepo, ILocationRepo locationRepo, IMapper mapper)
        {
            _buildingRepo = buildingRepo;
            _locationRepo = locationRepo;
            _mapper = mapper;
        }

        public async Task<List<BuildingReadDto>> GetAllBuildingsAsync()
        {
            var buildings = await _buildingRepo.GetAllBuildingsAsync();
            return _mapper.Map<List<BuildingReadDto>>(buildings);
        }

        public async Task<BuildingReadDto> GetBuildingByIdAsync(Guid id)
        {
            var building = await _buildingRepo.GetBuildingByIdAsync(id);
            return _mapper.Map<BuildingReadDto>(building);
        }        public async Task<BuildingReadDto> CreateNewBuildingAsync(CreateNewBuildingDto request)
        {
            // Check if location exists
            var location = await _locationRepo.GetLocationByIdAsync(request.LocationId);
            if (location == null)
            {
                throw new KeyNotFoundException($"Location with ID {request.LocationId} not found");
            }

            var building = new Building
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                LocationId = request.LocationId,
                IsActive = true
            };

            var createdBuilding = await _buildingRepo.CreateBuildingAsync(building);
            return _mapper.Map<BuildingReadDto>(createdBuilding);
        }

        public async Task<bool> DeactivateBuildingAsync(DeactivateDto request)
        {
            return await _buildingRepo.DeactivateBuildingAsync(request.Id);
        }

        public async Task<bool> ActivateBuildingAsync(ActivateDto request)
        {
            return await _buildingRepo.ActivateBuildingAsync(request.Id);
        }

        public async Task<BuildingListResponse> GetBuildingsByPaginationAsync(Pagination pagination, BuildingListFilterParams filter, Order? order)
        {
            var result = await _buildingRepo.GetBuildingsByPaginationAsync(pagination, filter, order);
            var buildingDtos = _mapper.Map<List<BuildingReadDto>>(result.Data);
            
            return new BuildingListResponse
            {
                Data = buildingDtos,
                Total = result.Total,
                PageSize = result.PageSize,
                PageIndex = result.PageIndex
            };
        }
    }
}
