using System.ComponentModel.DataAnnotations;

namespace UserService.Business.Dtos.Student
{
    public class StudentDto
    {
        public Guid Id { get; set; }
        [Required]
        public string StudentCode { get; set; }
        [Required]
        public Guid MajorId { get; set; }

        [Required]
        public Guid BatchId { get; set; }

        public ApplicationUserDto ApplicationUser { get; set; }
    }
} 