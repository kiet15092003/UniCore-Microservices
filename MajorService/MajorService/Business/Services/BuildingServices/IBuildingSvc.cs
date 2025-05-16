using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Building;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MajorService.Business.Services.BuildingServices
{    public interface IBuildingSvc
    {
        Task<List<BuildingReadDto>> GetAllBuildingsAsync();
        Task<BuildingReadDto> GetBuildingByIdAsync(Guid id);
        Task<BuildingReadDto> CreateNewBuildingAsync(CreateNewBuildingDto request);
        Task<BuildingReadDto> UpdateBuildingAsync(Guid id, UpdateBuildingDto request);
        Task<bool> DeactivateBuildingAsync(DeactivateDto request);
        Task<bool> ActivateBuildingAsync(ActivateDto request);
        Task<BuildingListResponse> GetBuildingsByPaginationAsync(Pagination pagination, BuildingListFilterParams filter, Order? order);
    }
}
