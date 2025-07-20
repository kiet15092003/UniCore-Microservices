using System.ComponentModel.DataAnnotations;

namespace UserService.Business.Dtos.Student
{
    public class CreateStudentDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime Dob { get; set; }

        [Required]
        public string PersonId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string PrivateEmail { get; set; }

        [Required]
        public Guid BatchId { get; set; }

        [Required]
        public Guid MajorId { get; set; }
    }
} 