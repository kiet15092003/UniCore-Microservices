using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace UserService.Entities
{
    public class Student : BaseEntity
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
        public Batch Batch { get; set; }

        public Guid? GuardianId { get; set; }
        public Guardian? Guardian { get; set; }

        [Required]
        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
