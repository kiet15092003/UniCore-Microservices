using System.ComponentModel.DataAnnotations;

namespace EnrollmentService.Business.Dtos.Exam
{
    public class AddEnrollmentToExamDto
    {
        [Required]
        public Guid ExamId { get; set; }
        
        [Required]
        public List<Guid> EnrollmentIds { get; set; } = new List<Guid>();
    }
}
