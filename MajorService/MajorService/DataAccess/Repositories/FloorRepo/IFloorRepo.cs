using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MajorService.DataAccess.Repositories.FloorRepo
{
    public interface IFloorRepo
    {
        Task<List<Floor>> GetAllFloorsAsync();
        Task<Floor?> GetFloorByIdAsync(Guid id);
        Task<Floor> CreateFloorAsync(Floor floor);
        Task<Floor> UpdateFloorAsync(Floor floor);
        Task<bool> DeactivateFloorAsync(Floor floor);
        Task<bool> ActivateFloorAsync(Floor floor);
        Task<(List<Floor> Data, int Count)> GetFloorsByPaginationAsync(
            Pagination pagination, 
            FloorListFilterParams filter, 
            Order? order);
    }
}
