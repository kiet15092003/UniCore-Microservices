using System.ComponentModel.DataAnnotations;

namespace UserService.Business.Dtos.Student
{
    public class UpdateStudentDto
    { 
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
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PersonId { get; set; }
        [Required]
        public DateTime Dob { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public int Status { get; set; }
       
    }

}
