using System.ComponentModel.DataAnnotations;

namespace CourseService.Business.Dtos.AcademicClass
{
    public class BulkChangeStatusDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one academic class ID must be provided")]
        public List<Guid> AcademicClassIds { get; set; } = new List<Guid>();
        
        [Required]
        public int Status { get; set; }
    }
} 