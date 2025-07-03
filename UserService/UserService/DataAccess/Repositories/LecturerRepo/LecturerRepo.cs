using Microsoft.EntityFrameworkCore;
using UserService.Entities;
using UserService.DataAccess;
using UserService.Business.Dtos.Lecturer;
using UserService.Utils.Pagination;
using UserService.Utils.Filter;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using AutoMapper;

namespace UserService.DataAccess.Repositories.LecturerRepo
{
    public class LecturerRepo : ILecturerRepo
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public LecturerRepo(AppDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Lecturer> GetLecturerByIdAsync(Guid id)
        {
            return await _context.Lecturers
                .Include(l => l.ApplicationUser)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Lecturer> GetLecturerDetailByIdAsync(Guid id)
        {
            return await _context.Lecturers
                .Include(l => l.ApplicationUser)
                .ThenInclude(a => a.Address)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        private IQueryable<Lecturer> ApplyFilter(IQueryable<Lecturer> query, LecturerListFilterParams filter)
        {
            if (!string.IsNullOrEmpty(filter.SearchQuery))
            {
                query = query.Where(l => 
                    l.LecturerCode.Contains(filter.SearchQuery) || 
                    l.ApplicationUser.Email.Contains(filter.SearchQuery) || 
                    l.ApplicationUser.FirstName.Contains(filter.SearchQuery) || 
                    l.ApplicationUser.LastName.Contains(filter.SearchQuery) ||
                    l.ApplicationUser.PhoneNumber.Contains(filter.SearchQuery));
            }

            if (filter.DepartmentId.HasValue && filter.DepartmentId != Guid.Empty)
            {
                query = query.Where(l => l.DepartmentId == filter.DepartmentId.Value);
            }

            if (filter.WorkingStatus.HasValue)
            {
                query = query.Where(l => l.WorkingStatus == filter.WorkingStatus.Value);
            }

            if (!string.IsNullOrEmpty(filter.MainMajor))
            {
                query = query.Where(l => l.MainMajor.Contains(filter.MainMajor));
            }

            if (!string.IsNullOrEmpty(filter.Degree))
            {
                query = query.Where(l => l.Degree.Contains(filter.Degree));
            }

            if (filter.Salary.HasValue)
            {
                query = query.Where(l => l.Salary == filter.Salary.Value);
            }

            if (filter.JoinDate.HasValue)
            {
                query = query.Where(l => l.JoinDate.Date == filter.JoinDate.Value.Date);
            }

            return query;
        }

        private IQueryable<Lecturer> ApplyOrder(IQueryable<Lecturer> query, Order order)
        {
            if (string.IsNullOrEmpty(order.By))
            {
                return query.OrderBy(l => l.LecturerCode);
            }

            switch (order.By.ToLower())
            {
                case "lecturercode":
                    return order.IsDesc ? query.OrderByDescending(l => l.LecturerCode) : query.OrderBy(l => l.LecturerCode);
                case "email":
                    return order.IsDesc ? query.OrderByDescending(l => l.ApplicationUser.Email) : query.OrderBy(l => l.ApplicationUser.Email);
                case "firstname":
                    return order.IsDesc ? query.OrderByDescending(l => l.ApplicationUser.FirstName) : query.OrderBy(l => l.ApplicationUser.FirstName);
                case "lastname":
                    return order.IsDesc ? query.OrderByDescending(l => l.ApplicationUser.LastName) : query.OrderBy(l => l.ApplicationUser.LastName);
                case "phonenumber":
                    return order.IsDesc ? query.OrderByDescending(l => l.ApplicationUser.PhoneNumber) : query.OrderBy(l => l.ApplicationUser.PhoneNumber);
                case "degree":
                    return order.IsDesc ? query.OrderByDescending(l => l.Degree) : query.OrderBy(l => l.Degree);
                case "joindate":
                    return order.IsDesc ? query.OrderByDescending(l => l.JoinDate) : query.OrderBy(l => l.JoinDate);
                default:
                    return order.IsDesc ? query.OrderByDescending(l => l.LecturerCode) : query.OrderBy(l => l.LecturerCode);
            }
        }

        public async Task<PaginationResult<LecturerDto>> GetAllPaginationAsync(Pagination pagination, LecturerListFilterParams filter, Order order)
        {
            var query = _context.Lecturers
                .Include(l => l.ApplicationUser)
                .AsQueryable();

            // Apply filter if needed
            if (filter != null)
            {
                query = ApplyFilter(query, filter);
            }

            // Apply ordering if needed
            if (order != null)
            {
                query = ApplyOrder(query, order);
            }

            // Get total count before applying pagination
            int total = await query.CountAsync();

            var result = await query
                .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                .Take(pagination.ItemsPerpage)
                .ToListAsync();

            var mappedResult = _mapper.Map<List<LecturerDto>>(result);

            return new PaginationResult<LecturerDto>
            {
                Data = mappedResult,
                Total = total,
                PageSize = pagination.ItemsPerpage,
                PageIndex = pagination.PageNumber
            };
        }

        public async Task<Lecturer> CreateAsync(Lecturer lecturer)
        {
            await _context.Lecturers.AddAsync(lecturer);
            await SaveChangesAsync();
            return lecturer;
        }

        public async Task<Lecturer> UpdateAsync(Lecturer lecturer)
        {
            _context.Lecturers.Update(lecturer);
            await SaveChangesAsync();
            return lecturer;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var lecturer = await GetLecturerByIdAsync(id);
            if (lecturer == null)
            {
                return false;
            }

            // Get the application user ID
            var userId = lecturer.ApplicationUserId;

            // Remove lecturer first
            _context.Lecturers.Remove(lecturer);
            await SaveChangesAsync();

            // Remove the application user
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await SaveChangesAsync();
            }

            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<(List<ApplicationUser> Users, List<Lecturer> Lecturers)> AddLecturersWithUsersAsync(
            List<(ApplicationUser User, Lecturer Lecturer)> userLecturerPairs,
            Dictionary<string, string>? passwords = null)
        {
            var users = new List<ApplicationUser>();
            var lecturers = new List<Lecturer>();

            foreach (var pair in userLecturerPairs)
            {
                var (user, lecturer) = pair;

                // Create the user first
                if (passwords != null && passwords.TryGetValue(user.Email, out var password))
                {
                    var result = await _userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        users.Add(user);

                        // Link the lecturer to the created user
                        lecturer.ApplicationUserId = user.Id;
                        await _context.Lecturers.AddAsync(lecturer);
                        lecturers.Add(lecturer);
                    }
                }
                else
                {
                    // For scenarios without password (like bulk imports)
                    await _context.Users.AddAsync(user);
                    users.Add(user);

                    lecturer.ApplicationUserId = user.Id;
                    await _context.Lecturers.AddAsync(lecturer);
                    lecturers.Add(lecturer);
                }
            }

            await SaveChangesAsync();
            return (users, lecturers);
        }

        public async Task<string> UpdateLecturerImageAsync(Guid id, string imageUrl)
        {
            var lecturer = await GetLecturerByIdAsync(id);
            if (lecturer != null)
            {
                var user = lecturer.ApplicationUser;
                user.ImageUrl = imageUrl;
                await SaveChangesAsync();
                return imageUrl;
            }
            return null;
        }

        public async Task<Lecturer> GetLecturerByEmailAsync(string email)
        {
            return await _context.Lecturers
                .Include(l => l.ApplicationUser)
                .FirstOrDefaultAsync(l => l.ApplicationUser.Email == email);
        }

        public async Task<List<Lecturer>> GetLecturersByDepartmentIdsAsync(List<Guid> departmentIds)
        {
            return await _context.Lecturers
                .Include(l => l.ApplicationUser)
                .Where(l => departmentIds.Contains(l.DepartmentId))
                .ToListAsync();
        }
    }
} 