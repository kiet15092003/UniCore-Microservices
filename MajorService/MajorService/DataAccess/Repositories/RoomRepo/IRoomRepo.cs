using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MajorService.DataAccess.Repositories.RoomRepo
{
    public interface IRoomRepo
    {
        Task<List<Room>> GetAllRoomsAsync();
        Task<Room?> GetRoomByIdAsync(Guid id);
        Task<Room> CreateRoomAsync(Room room);
        Task<bool> DeactivateRoomAsync(Room room);
        Task<bool> ActivateRoomAsync(Room room);
        Task<(List<Room> Data, int Count)> GetRoomsByPaginationAsync(
            Pagination pagination, 
            RoomListFilterParams filter, 
            Order? order);
    }
}
