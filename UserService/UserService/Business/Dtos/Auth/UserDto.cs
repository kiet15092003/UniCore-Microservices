using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserService.Business.Dtos.Auth
{
    public class UserDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PersonId { get; set; }
        [Required]
        public DateTime Dob { get; set; }
        [Required]
        public int Status { get; set; } = 1;
        public string ImageUrl { get; set; } = "";
        public string Role { get; set; } = "student";
    }
}
