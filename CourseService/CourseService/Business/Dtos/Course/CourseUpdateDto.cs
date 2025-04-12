using System.ComponentModel.DataAnnotations;

namespace CourseService.Business.Dtos.Course
{
    public class CourseUpdateDto
    {
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public decimal Price { get; set; }
        
        public Guid? MajorId { get; set; }
        
        public bool IsActive { get; set; }
    }
} 