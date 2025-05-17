using Microsoft.EntityFrameworkCore;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;
using MajorService.Business.Dtos.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MajorService.DataAccess.Repositories.BuildingRepo
{
    public class BuildingRepo : IBuildingRepo
    {
        private readonly AppDbContext _context;

        public BuildingRepo(AppDbContext context)
        {
            _context = context;
        }

    public async Task<List<Building>> GetAllBuildingsAsync()
    {
        return await _context.Buildings
            .Include(b => b.Floors)
                .ThenInclude(f => f.Rooms)
            .ToListAsync();
    }    
    
    public async Task<Building> GetBuildingByIdAsync(Guid id)
    {
        return await _context.Buildings
            .Include(b => b.Floors)
                .ThenInclude(f => f.Rooms)
            .FirstOrDefaultAsync(b => b.Id == id) ?? new Building();
    }

        public async Task<Building> CreateBuildingAsync(Building building)
        {
            _context.Buildings.Add(building);
            await _context.SaveChangesAsync();
            return building;
        }

        public async Task<bool> UpdateBuildingAsync(Building building)
        {
            var existingBuilding = await _context.Buildings.FindAsync(building.Id);
            if (existingBuilding == null) return false;

            _context.Entry(existingBuilding).CurrentValues.SetValues(building);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateBuildingAsync(Guid id)
        {
            var building = await _context.Buildings.FindAsync(id);
            if (building == null) return false;

            building.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateBuildingAsync(Guid id)
        {
            var building = await _context.Buildings.FindAsync(id);
            if (building == null) return false;

            building.IsActive = true;
            await _context.SaveChangesAsync();
            return true;
        }        private IQueryable<Building> ApplyFilters(IQueryable<Building> query, BuildingListFilterParams filterParams)
        {
            if (!string.IsNullOrEmpty(filterParams.Name))
            {
                query = query.Where(b => b.Name.Contains(filterParams.Name));
            }
            
            if (filterParams.LocationId.HasValue)
            {
                query = query.Where(b => b.LocationId == filterParams.LocationId.Value);
            }

            return query;
        }

        private IQueryable<Building> ApplySorting(IQueryable<Building> query, Order? order)
        {
            if (order != null && !string.IsNullOrEmpty(order.By))
            {
                query = order.IsDesc
                    ? query.OrderByDescending(e => EF.Property<object>(e, order.By))
                    : query.OrderBy(e => EF.Property<object>(e, order.By));
            }

            return query;
        }    public async Task<PaginationResult<Building>> GetBuildingsByPaginationAsync(
            Pagination pagination,
            BuildingListFilterParams filterParams,
            Order? order)
        {
            var query = _context.Buildings
                .Include(b => b.Location)
                .Include(b => b.Floors)
                    .ThenInclude(f => f.Rooms)
                .AsQueryable();

            query = ApplyFilters(query, filterParams);
            query = ApplySorting(query, order);

            var total = await query.CountAsync();
            var data = await query.Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                                   .Take(pagination.ItemsPerpage)
                                   .ToListAsync();

            return new PaginationResult<Building>
            {
                Data = data,
                Total = total,
                PageSize = pagination.ItemsPerpage,
                PageIndex = pagination.PageNumber
            };
        }
    }
}
