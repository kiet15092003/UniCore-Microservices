using AutoMapper;
using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Floor;
using MajorService.DataAccess.Repositories.BuildingRepo;
using MajorService.DataAccess.Repositories.FloorRepo;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MajorService.Business.Services.FloorServices
{
    public class FloorSvc : IFloorSvc
    {
        private readonly IFloorRepo _floorRepo;
        private readonly IBuildingRepo _buildingRepo;
        private readonly IMapper _mapper;

        public FloorSvc(IFloorRepo floorRepo, IBuildingRepo buildingRepo, IMapper mapper)
        {
            _floorRepo = floorRepo;
            _buildingRepo = buildingRepo;
            _mapper = mapper;
        }

        public async Task<bool> ActivateFloorAsync(ActivateDto request)
        {
            var floor = await _floorRepo.GetFloorByIdAsync(request.Id);
            if (floor == null)
            {
                throw new KeyNotFoundException($"Floor with ID {request.Id} not found");
            }
            
            return await _floorRepo.ActivateFloorAsync(floor);
        }

        public async Task<FloorReadDto> CreateNewFloorAsync(CreateNewFloorDto request)
        {
            // Check if building exists
            var building = await _buildingRepo.GetBuildingByIdAsync(request.BuildingId);
            if (building == null)
            {
                throw new KeyNotFoundException($"Building with ID {request.BuildingId} not found");
            }

            // Create new floor
            var floor = _mapper.Map<Floor>(request);
            floor.IsActive = true;
            
            await _floorRepo.CreateFloorAsync(floor);
            
            return _mapper.Map<FloorReadDto>(floor);
        }

        public async Task<bool> DeactivateFloorAsync(DeactivateDto request)
        {
            var floor = await _floorRepo.GetFloorByIdAsync(request.Id);
            if (floor == null)
            {
                throw new KeyNotFoundException($"Floor with ID {request.Id} not found");
            }
            
            return await _floorRepo.DeactivateFloorAsync(floor);
        }

        public async Task<List<FloorReadDto>> GetAllFloorsAsync()
        {
            var floors = await _floorRepo.GetAllFloorsAsync();
            return _mapper.Map<List<FloorReadDto>>(floors);
        }        
        public async Task<FloorReadDto> GetFloorByIdAsync(Guid id)
        {
            var floor = await _floorRepo.GetFloorByIdAsync(id);
            if (floor == null)
            {
                throw new KeyNotFoundException($"Floor with ID {id} not found");
            }
            
            return _mapper.Map<FloorReadDto>(floor);
        }
        
        public async Task<FloorReadDto> UpdateFloorAsync(Guid id, UpdateFloorDto request)
        {
            var floor = await _floorRepo.GetFloorByIdAsync(id);
            if (floor == null)
            {
                throw new KeyNotFoundException($"Floor with ID {id} not found");
            }
            
            floor.Name = request.Name;
            
            await _floorRepo.UpdateFloorAsync(floor);
            
            return _mapper.Map<FloorReadDto>(floor);
        }

        public async Task<FloorListResponse> GetFloorsByPaginationAsync(
            Pagination pagination, 
            FloorListFilterParams filter, 
            Order? order)
        {
            var (floors, total) = await _floorRepo.GetFloorsByPaginationAsync(pagination, filter, order);
              return new FloorListResponse
            {
                Data = _mapper.Map<List<FloorReadDto>>(floors),
                Total = total,
                PageIndex = pagination.PageNumber,
                PageSize = pagination.ItemsPerpage
            };
        }

        public async Task<List<FloorReadDto>> GetFloorsByLocationIdAsync(Guid locationId)
        {
            var floors = await _floorRepo.GetAllFloorsAsync();
            var filteredFloors = new List<Floor>();
            
            foreach (var floor in floors)
            {
                var building = await _buildingRepo.GetBuildingByIdAsync(floor.BuildingId);
                if (building.LocationId == locationId)
                {
                    filteredFloors.Add(floor);
                }
            }
            
            return _mapper.Map<List<FloorReadDto>>(filteredFloors);
        }
    }
}
