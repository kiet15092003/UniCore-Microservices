using Microsoft.AspNetCore.Mvc;
using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Room;
using MajorService.Business.Services.RoomServices;
using MajorService.Entities;
using MajorService.Middleware;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MajorService.Controller
{
    [ApiController]
    [Route("api/m/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomSvc _roomSvc;

        public RoomController(IRoomSvc roomSvc)
        {
            _roomSvc = roomSvc;
        }

        [HttpGet]
        public async Task<ApiResponse<List<RoomReadDto>>> GetAllRoomsAsync()
        {
            var rooms = await _roomSvc.GetAllRoomsAsync();
            return ApiResponse<List<RoomReadDto>>.SuccessResponse(rooms);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<RoomReadDto>> GetRoomByIdAsync(Guid id)
        {
            var room = await _roomSvc.GetRoomByIdAsync(id);
            return ApiResponse<RoomReadDto>.SuccessResponse(room);
        }

        [HttpPost]
        public async Task<ApiResponse<RoomReadDto>> CreateNewRoomAsync([FromBody] CreateNewRoomDto request)
        {
            try
            {
                var room = await _roomSvc.CreateNewRoomAsync(request);
                return ApiResponse<RoomReadDto>.SuccessResponse(room);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<RoomReadDto>.ErrorResponse(new List<string> { ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return ApiResponse<RoomReadDto>.ErrorResponse(new List<string> { ex.Message });
            }
        }

        [HttpPost("{id}/deactivate")]
        public async Task<ApiResponse<bool>> DeactivateRoomAsync(Guid id)
        {
            var deactivateDto = new DeactivateDto { Id = id };
            var result = await _roomSvc.DeactivateRoomAsync(deactivateDto);
            if (result)
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }
            return ApiResponse<bool>.ErrorResponse(new List<string> { "Failed to deactivate room" });
        }

        [HttpPost("{id}/activate")]
        public async Task<ApiResponse<bool>> ActivateRoomAsync(Guid id)
        {
            var activateDto = new ActivateDto { Id = id };
            var result = await _roomSvc.ActivateRoomAsync(activateDto);
            if (result)
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }
            return ApiResponse<bool>.ErrorResponse(new List<string> { "Failed to activate room" });
        }        [HttpGet("page")]
        public async Task<ApiResponse<RoomListResponse>> GetByPagination([FromQuery] GetRoomByPaginationDto param)
        {
            var result = await _roomSvc.GetRoomsByPaginationAsync(param.Pagination, param.Filter, param.Order);
            return ApiResponse<RoomListResponse>.SuccessResponse(result);
        }
    }
}
