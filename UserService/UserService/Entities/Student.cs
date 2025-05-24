using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

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

        public List<Guardian> Guardians { get; set; }

        [Required]
        public String ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
