using CourseService.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CourseService.DataAccess.Repositories.MajorRepo
{
    public interface IMajorRepo
    {
        Task<IActionResult> CreateMajorAsync(Major major);
    }
}
