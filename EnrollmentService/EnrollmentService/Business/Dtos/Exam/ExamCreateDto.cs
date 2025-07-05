using System.ComponentModel.DataAnnotations;

namespace EnrollmentService.Business.Dtos.Exam
{
    public class ExamCreateDto
    {
        [Required]
        public int Group { get; set; }
        
        [Required]
        public int Type { get; set; }
        
        [Required]
        public DateTime ExamTime { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0")]
        public int Duration { get; set; }
        
        [Required]
        public Guid AcademicClassId { get; set; }
        
        [Required]
        public Guid RoomId { get; set; }
    }
}
