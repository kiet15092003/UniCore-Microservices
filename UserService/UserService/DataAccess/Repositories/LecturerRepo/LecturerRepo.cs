using UserService.Entities;

namespace UserService.DataAccess.Repositories.LecturerRepo
{
    public class LecturerRepo : ILecturerRepo
    {
        private readonly AppDbContext _context;
        public LecturerRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Lecturer> CreateLecturerAsync(Lecturer lecturer)
        {
            await _context.Lecturers.AddAsync(lecturer);
            await _context.SaveChangesAsync();
            return lecturer;
        }
    }
}
