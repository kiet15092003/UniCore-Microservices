using Microsoft.AspNetCore.Http;

namespace CourseService.Business.Dtos.Material
{
    public class CourseMaterialUpdateDto
    {
        public Guid MaterialId { get; set; }
        public string Name { get; set; }
        public IFormFile File { get; set; }
    }
} 