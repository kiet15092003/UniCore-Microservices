using System.ComponentModel.DataAnnotations;

namespace EnrollmentService.Business.Dtos.Enrollment
{
    public class MoveEnrollmentsDto
    {
        [Required]
        public List<Guid> EnrollmentIds { get; set; } = new();
        
        [Required]
        public Guid ToClassId { get; set; }
    }
}
