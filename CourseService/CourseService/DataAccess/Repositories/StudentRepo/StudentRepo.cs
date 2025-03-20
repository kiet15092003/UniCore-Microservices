using CourseService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseService.DataAccess.Repositories.StudentRepo
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

        public async Task<IEnumerable<Student>> GetAllStudentAsync()
        {
            return await _context.Students.ToListAsync(); 
        }
    }
}
