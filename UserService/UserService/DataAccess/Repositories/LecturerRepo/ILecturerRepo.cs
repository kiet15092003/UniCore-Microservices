using UserService.Entities;

namespace UserService.DataAccess.Repositories.LecturerRepo
{
    public interface ILecturerRepo
    {
        Task<Lecturer> CreateLecturerAsync(Lecturer lecturer);
    }
}
