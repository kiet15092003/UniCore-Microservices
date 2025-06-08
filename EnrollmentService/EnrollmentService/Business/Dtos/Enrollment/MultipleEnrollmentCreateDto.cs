using System.ComponentModel.DataAnnotations;

namespace EnrollmentService.Business.Dtos.Enrollment
{
    public class MultipleEnrollmentCreateDto
    {
        [Required]
        public Guid StudentId { get; set; }
        
        [Required]
        public List<Guid> AcademicClassIds { get; set; } = new();
    }
}
