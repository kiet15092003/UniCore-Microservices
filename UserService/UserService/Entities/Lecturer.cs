using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserService.Entities
{
    public class Lecturer : BaseEntity
    {

        [Required]
        public string LecturerCode { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
