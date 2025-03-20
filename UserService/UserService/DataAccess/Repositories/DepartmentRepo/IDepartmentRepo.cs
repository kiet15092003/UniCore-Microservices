using UserService.Entities;

namespace UserService.DataAccess.Repositories.DepartmentRepo
{
    public interface IDepartmentRepo
    {
        Task<Department> GetDepartmentByIdAsync(Guid Id);
    }
}
