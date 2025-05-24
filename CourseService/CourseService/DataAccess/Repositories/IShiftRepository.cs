using CourseService.Entities;

namespace CourseService.DataAccess.Repositories
{
    public interface IShiftRepository
    {
        Task<IEnumerable<Shift>> GetAllAsync();
        Task<Shift> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
