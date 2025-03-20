using System.ComponentModel.DataAnnotations;

namespace CourseService.Entities
{
    public class Student : BaseEntity
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string StudentCode { get; set; }
        [Required]
        public string FullName { get; set; }
    }
}
