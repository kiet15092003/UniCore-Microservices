using System.ComponentModel.DataAnnotations;

namespace UserService.Business.Dtos.Student
{
    public class UpdateStudentDto
    { 
        [Required]
        public string StudentCode { get; set; }
        [Required]
        public int AccumulateCredits { get; set; }
        [Required]
        public double AccumulateScore { get; set; }
        [Required]
        public int AccumulateActivityScore { get; set; }
        [Required]
        public Guid MajorId { get; set; }
        [Required]
        public Guid BatchId { get; set; }
        public Guid? GuardianId { get; set; }
    }

}
