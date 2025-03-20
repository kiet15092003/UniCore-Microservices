using System.ComponentModel.DataAnnotations;

namespace UserService.Entities
{
    public class TrainingManager : BaseEntity
    {
        public string TrainingManagerCode { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
