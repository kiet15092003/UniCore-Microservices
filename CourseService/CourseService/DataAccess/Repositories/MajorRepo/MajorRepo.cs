using CourseService.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CourseService.DataAccess.Repositories.MajorRepo
{
    public class MajorRepo : IMajorRepo
    {
        private readonly AppDbContext _context;
        public MajorRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CreateMajorAsync(Major major)
        {
            await _context.Majors.AddAsync(major);
            await _context.SaveChangesAsync();
            return new OkObjectResult(major);
        }
    }
}
