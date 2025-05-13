using System.ComponentModel.DataAnnotations;

namespace UserService.Business.Dtos.Student
{
    public class StudentDto
    {
        [Required]
        public string StudentCode { get; set; }
        [Required]
        public Guid MajorId { get; set; }

        [Required]
        public Guid BatchId { get; set; }

        [Required]
        public Guid ApplicationUserId { get; set; }

        public ApplicationUserDto ApplicationUser { get; set; }
    }
} 