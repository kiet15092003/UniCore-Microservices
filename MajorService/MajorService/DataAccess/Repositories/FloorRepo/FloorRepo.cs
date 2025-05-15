using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MajorService.DataAccess.Repositories.FloorRepo
{
    public class FloorRepo : IFloorRepo
    {
        private readonly AppDbContext _context;

        public FloorRepo(AppDbContext context)
        {
            _context = context;
        }        
        public async Task<List<Floor>> GetAllFloorsAsync()
        {
            return await _context.Floors
                .Include(f => f.Rooms)
                .ToListAsync();
        }

        public async Task<Floor?> GetFloorByIdAsync(Guid id)
        {
            return await _context.Floors
                .Include(f => f.Rooms)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Floor> CreateFloorAsync(Floor floor)
        {
            await _context.Floors.AddAsync(floor);
            await _context.SaveChangesAsync();
            return floor;
        }

        public async Task<bool> DeactivateFloorAsync(Floor floor)
        {
            floor.IsActive = false;
            _context.Floors.Update(floor);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ActivateFloorAsync(Floor floor)
        {
            floor.IsActive = true;
            _context.Floors.Update(floor);
            return await _context.SaveChangesAsync() > 0;
        }        private IQueryable<Floor> ApplyFilters(IQueryable<Floor> query, FloorListFilterParams filterParams)
        {
            if (!string.IsNullOrEmpty(filterParams.Name))
            {
                query = query.Where(f => f.Name.Contains(filterParams.Name));
            }
            
            if (filterParams.BuildingId.HasValue)
            {
                query = query.Where(f => f.BuildingId == filterParams.BuildingId.Value);
            }
            
            if (filterParams.IsActive.HasValue)
            {
                query = query.Where(f => f.IsActive == filterParams.IsActive.Value);
            }

            return query;
        }

        private IQueryable<Floor> ApplySorting(IQueryable<Floor> query, Order? order)
        {
            if (order != null && !string.IsNullOrEmpty(order.By))
            {
                query = order.IsDesc
                    ? query.OrderByDescending(e => EF.Property<object>(e, order.By))
                    : query.OrderBy(e => EF.Property<object>(e, order.By));
            }
            else
            {
                query = query.OrderBy(f => f.Name);
            }

            return query;
        }

        public async Task<(List<Floor> Data, int Count)> GetFloorsByPaginationAsync(
            Pagination pagination, 
            FloorListFilterParams filter, 
            Order? order)
        {
            var query = _context.Floors.Include(b => b.Building).AsQueryable();

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
    }
}
