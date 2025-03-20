using Microsoft.EntityFrameworkCore;
using StudentService.Entities;

namespace StudentService.DataAccess.Repositories.StudentRepo
{
    public class StudentRepo : IStudentRepo
    {
        private readonly AppDbContext _context;
        public StudentRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Student> CreateStudentAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
            return student;
        }
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Students.ToListAsync();
        }
    }
}
