using System.ComponentModel.DataAnnotations;

namespace StudentService.Entities
{
    public class Student : BaseEntity
    {
        [Required]
        public string StudentCode { get; set; }
        [Required]
        public int TotalCredits { get; set; }

        [Required]
        public Guid MajorId { get; set; }

        [Required]
        public Guid BatchId { get; set; }
        public Batch Batch { get; set; }

        [Required]
        public Guid ApplicationUserId { get; set; }
    }
}
