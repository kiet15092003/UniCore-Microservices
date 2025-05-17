using MajorService.Business.Dtos.Building;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MajorService.DataAccess.Repositories.BuildingRepo
{
    public interface IBuildingRepo
    {
        Task<List<Building>> GetAllBuildingsAsync();
        Task<Building> GetBuildingByIdAsync(Guid id);
        Task<Building> CreateBuildingAsync(Building building);
        Task<bool> UpdateBuildingAsync(Building building);
        Task<bool> DeactivateBuildingAsync(Guid id);
        Task<bool> ActivateBuildingAsync(Guid id);
        Task<PaginationResult<Building>> GetBuildingsByPaginationAsync(
            Pagination pagination,
            BuildingListFilterParams filterParams,
            Order? order);
    }
}
