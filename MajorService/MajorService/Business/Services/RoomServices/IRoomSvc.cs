using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Room;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MajorService.Business.Services.RoomServices
{    
    public interface IRoomSvc
    {
        Task<List<RoomReadDto>> GetAllRoomsAsync();
        Task<RoomReadDto> GetRoomByIdAsync(Guid id);
        Task<RoomReadDto> CreateNewRoomAsync(CreateNewRoomDto request);
        Task<bool> DeactivateRoomAsync(DeactivateDto request);
        Task<bool> ActivateRoomAsync(ActivateDto request);
        Task<RoomListResponse> GetRoomsByPaginationAsync(
            Pagination pagination, 
            RoomListFilterParams filter, 
            Order? order);
    }
}
