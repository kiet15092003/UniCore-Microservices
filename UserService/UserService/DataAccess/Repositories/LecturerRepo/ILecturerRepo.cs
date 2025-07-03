using UserService.Entities;
using UserService.Business.Dtos.Lecturer;
using UserService.Utils.Pagination;
using UserService.Utils.Filter;

namespace UserService.DataAccess.Repositories.LecturerRepo
{
    public interface ILecturerRepo
    {
        Task<Lecturer> GetLecturerByIdAsync(Guid id);
        Task<PaginationResult<LecturerDto>> GetAllPaginationAsync(Pagination pagination, LecturerListFilterParams filter, Order order);
        Task<Lecturer> CreateAsync(Lecturer lecturer);
        Task<Lecturer> UpdateAsync(Lecturer lecturer);
        Task<bool> DeleteAsync(Guid id);
        Task SaveChangesAsync();
        Task<(List<ApplicationUser> Users, List<Lecturer> Lecturers)> AddLecturersWithUsersAsync(
            List<(ApplicationUser User, Lecturer Lecturer)> userLecturerPairs,
            Dictionary<string, string>? passwords = null);
        Task<string> UpdateLecturerImageAsync(Guid id, string imageUrl);
        Task<Lecturer> GetLecturerDetailByIdAsync(Guid id);
        Task<Lecturer> GetLecturerByEmailAsync(string email);
        Task<List<Lecturer>> GetLecturersByDepartmentIdsAsync(List<Guid> departmentIds);
    }
} 