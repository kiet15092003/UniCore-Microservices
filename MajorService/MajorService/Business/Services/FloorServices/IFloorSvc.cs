using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Floor;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MajorService.Business.Services.FloorServices
{    
    public interface IFloorSvc
    {
        Task<List<FloorReadDto>> GetAllFloorsAsync();
        Task<FloorReadDto> GetFloorByIdAsync(Guid id);
        Task<FloorReadDto> CreateNewFloorAsync(CreateNewFloorDto request);
        Task<FloorReadDto> UpdateFloorAsync(Guid id, UpdateFloorDto request);
        Task<bool> DeactivateFloorAsync(DeactivateDto request);
        Task<bool> ActivateFloorAsync(ActivateDto request);
        Task<FloorListResponse> GetFloorsByPaginationAsync(Pagination pagination, FloorListFilterParams filter, Order? order);
    }
}
