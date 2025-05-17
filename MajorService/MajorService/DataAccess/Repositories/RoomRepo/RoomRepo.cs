using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MajorService.DataAccess.Repositories.RoomRepo
{
    public class RoomRepo : IRoomRepo
    {
        private readonly AppDbContext _context;

        public RoomRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms.ToListAsync();
        }

        public async Task<Room?> GetRoomByIdAsync(Guid id)
        {
            return await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Room> CreateRoomAsync(Room room)
        {
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<bool> DeactivateRoomAsync(Room room)
        {
            room.IsActive = false;
            _context.Rooms.Update(room);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ActivateRoomAsync(Room room)
        {
            room.IsActive = true;
            _context.Rooms.Update(room);
            return await _context.SaveChangesAsync() > 0;
        }        private IQueryable<Room> ApplyFilters(IQueryable<Room> query, RoomListFilterParams filterParams)
        {
            if (!string.IsNullOrEmpty(filterParams.Name))
            {
                query = query.Where(r => r.Name.Contains(filterParams.Name));
            }
            
            if (filterParams.FloorId.HasValue)
            {
                query = query.Where(r => r.FloorId == filterParams.FloorId.Value);
            }
            
            if (filterParams.BuildingId.HasValue)
            {
                query = query.Where(r => r.Floor!.BuildingId == filterParams.BuildingId.Value);
            }
            
            if (filterParams.LocationId.HasValue)
            {
                query = query.Where(r => r.Floor!.Building!.LocationId == filterParams.LocationId.Value);
            }
            
            if (filterParams.IsActive.HasValue)
            {
                query = query.Where(r => r.IsActive == filterParams.IsActive.Value);
            }
            
            if (filterParams.AvailableSeats.HasValue)
            {
                query = query.Where(r => r.AvailableSeats >= filterParams.AvailableSeats.Value);
            }

            return query;
        }

        private IQueryable<Room> ApplySorting(IQueryable<Room> query, Order? order)
        {
            if (order != null && !string.IsNullOrEmpty(order.By))
            {
                query = order.IsDesc
                    ? query.OrderByDescending(e => EF.Property<object>(e, order.By))
                    : query.OrderBy(e => EF.Property<object>(e, order.By));
            }
            else
            {
                query = query.OrderBy(r => r.Name);
            }

            return query;
        }        
        public async Task<(List<Room> Data, int Count)> GetRoomsByPaginationAsync(
            Pagination pagination, 
            RoomListFilterParams filter, 
            Order? order)
        {
            var query = _context.Rooms
                .Include(r => r.Floor!).ThenInclude(f => f.Building)
                .AsQueryable();

            query = ApplyFilters(query, filter);
            query = ApplySorting(query, order);

            // Get total count
            var count = await query.CountAsync();

            // Apply pagination
            var data = await query
                .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                .Take(pagination.ItemsPerpage)
                .ToListAsync();

            return (data, count);
        }

        public async Task<Room> UpdateRoomAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
            return room;
        }
    }
}
