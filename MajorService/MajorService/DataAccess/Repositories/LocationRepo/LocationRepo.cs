using Microsoft.EntityFrameworkCore;
using MajorService.Entities;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.DataAccess.Repositories.LocationRepo
{
    public class LocationRepo : ILocationRepo
    {
        private readonly AppDbContext _context;

        public LocationRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Location>> GetAllLocationsAsync()
        {
            return await _context.Locations.ToListAsync();
        }        public async Task<Location> GetLocationByIdAsync(Guid id)
        {
            return await _context.Locations.FindAsync(id) ?? throw new ArgumentException($"Location with ID {id} not found");
        }

        public async Task<Location> CreateLocationAsync(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task<bool> DeactivateLocationAsync(Guid id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null) return false;

            location.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateLocationAsync(Guid id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null) return false;

            location.IsActive = true;
            await _context.SaveChangesAsync();
            return true;
        }

        private IQueryable<Location> ApplyFilters(IQueryable<Location> query, LocationListFilterParams filterParams)
        {
            if (!string.IsNullOrEmpty(filterParams.Name))
            {
                query = query.Where(l => l.Name.Contains(filterParams.Name));
            }

            return query;
        }

        private IQueryable<Location> ApplySorting(IQueryable<Location> query, Order? order)
        {
            if (order != null && !string.IsNullOrEmpty(order.By))
            {
                query = order.IsDesc
                    ? query.OrderByDescending(e => EF.Property<object>(e, order.By))
                    : query.OrderBy(e => EF.Property<object>(e, order.By));
            }

            return query;
        }

        public async Task<PaginationResult<Location>> GetLocationsByPaginationAsync(
            Pagination pagination,
            LocationListFilterParams filterParams,
            Order? order)
        {
            var query = _context.Locations.AsQueryable();

            query = ApplyFilters(query, filterParams);
            query = ApplySorting(query, order);

            var total = await query.CountAsync();
            var data = await query.Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                                   .Take(pagination.ItemsPerpage)
                                   .ToListAsync();

            return new PaginationResult<Location>
            {
                Data = data,
                Total = total,
                PageSize = pagination.ItemsPerpage,
                PageIndex = pagination.PageNumber
            };
        }

        public async Task<int> GetTotalBuildingsForLocationAsync(Guid locationId)
        {
            return await _context.Buildings
                .Where(b => b.LocationId == locationId && b.IsActive)
                .CountAsync();
        }

        public async Task<int> GetTotalFloorsForLocationAsync(Guid locationId)
        {
            return await _context.Floors
                .Include(f => f.Building)
                .Where(f => f.Building.LocationId == locationId && f.IsActive)
                .CountAsync();
        }

        public async Task<int> GetTotalRoomForLocationAsync(Guid locationId)
        {
            return await _context.Rooms
                .Include(r => r.Floor)
                .ThenInclude(f => f.Building)
                .Where(r => r.Floor.Building.LocationId == locationId && r.IsActive)
                .CountAsync();
        }
    }
}
