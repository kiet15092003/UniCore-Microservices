using Microsoft.AspNetCore.Http;

namespace CourseService.Business.Dtos.Material
{
    public class CourseMaterialCreateDto
    {
        public Guid CourseId { get; set; }
        public string Name { get; set; }
        public IFormFile File { get; set; }
    }
} 