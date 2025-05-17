using AutoMapper;
using MajorService.Business.Dtos;
using MajorService.Business.Dtos.Room;
using MajorService.DataAccess.Repositories.FloorRepo;
using MajorService.DataAccess.Repositories.RoomRepo;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MajorService.Business.Services.RoomServices
{
    public class RoomSvc : IRoomSvc
    {
        private readonly IRoomRepo _roomRepo;
        private readonly IFloorRepo _floorRepo;
        private readonly IMapper _mapper;

        public RoomSvc(IRoomRepo roomRepo, IFloorRepo floorRepo, IMapper mapper)
        {
            _roomRepo = roomRepo;
            _floorRepo = floorRepo;
            _mapper = mapper;
        }

        public async Task<bool> ActivateRoomAsync(ActivateDto request)
        {
            var room = await _roomRepo.GetRoomByIdAsync(request.Id);
            if (room == null)
            {
                throw new KeyNotFoundException($"Room with ID {request.Id} not found");
            }
            
            return await _roomRepo.ActivateRoomAsync(room);
        }

        public async Task<RoomReadDto> CreateNewRoomAsync(CreateNewRoomDto request)
        {
            // Check if floor exists
            var floor = await _floorRepo.GetFloorByIdAsync(request.FloorId);
            if (floor == null)
            {
                throw new KeyNotFoundException($"Floor with ID {request.FloorId} not found");
            }

            // Create new room
            var room = _mapper.Map<Room>(request);
            room.IsActive = true;
            
            await _roomRepo.CreateRoomAsync(room);
            
            return _mapper.Map<RoomReadDto>(room);
        }

        public async Task<bool> DeactivateRoomAsync(DeactivateDto request)
        {
            var room = await _roomRepo.GetRoomByIdAsync(request.Id);
            if (room == null)
            {
                throw new KeyNotFoundException($"Room with ID {request.Id} not found");
            }
            
            return await _roomRepo.DeactivateRoomAsync(room);
        }

        public async Task<List<RoomReadDto>> GetAllRoomsAsync()
        {
            var rooms = await _roomRepo.GetAllRoomsAsync();
            return _mapper.Map<List<RoomReadDto>>(rooms);
        }

        public async Task<RoomReadDto> GetRoomByIdAsync(Guid id)
        {
            var room = await _roomRepo.GetRoomByIdAsync(id);
            if (room == null)
            {
                throw new KeyNotFoundException($"Room with ID {id} not found");
            }
            
            return _mapper.Map<RoomReadDto>(room);
        }

        public async Task<RoomListResponse> GetRoomsByPaginationAsync(
            Pagination pagination, 
            RoomListFilterParams filter, 
            Order? order)
        {
            var (rooms, total) = await _roomRepo.GetRoomsByPaginationAsync(pagination, filter, order);
            
            return new RoomListResponse
            {
                Data = _mapper.Map<List<RoomReadDto>>(rooms),
                Total = total,
                PageIndex = pagination.PageNumber,
                PageSize = pagination.ItemsPerpage
            };
        }

        public async Task<RoomReadDto> UpdateRoomAsync(UpdateRoomDto request)
        {
            var room = await _roomRepo.GetRoomByIdAsync(request.Id);
            if (room == null)
            {
                throw new KeyNotFoundException($"Room with ID {request.Id} not found");
            }

            // Update room properties
            if (!string.IsNullOrEmpty(request.Name))
            {
                room.Name = request.Name;
            }

            if (request.AvailableSeats.HasValue)
            {
                room.AvailableSeats = request.AvailableSeats.Value;
            }

            await _roomRepo.UpdateRoomAsync(room);
            
            return _mapper.Map<RoomReadDto>(room);
        }
    }
}
