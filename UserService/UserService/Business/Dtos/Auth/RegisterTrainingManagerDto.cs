using System.ComponentModel.DataAnnotations;

namespace UserService.Business.Dtos.Auth
{
    public class RegisterTrainingManagerDto
    {
        [Required]
        public string FullName { get; set; }

        [Required, Phone]
        public string PhoneNumber { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 16 characters.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,16}$",
                ErrorMessage = "Password must contain at least one letter, one number, and one special character.")]
        public string Password { get; set; }

        [Required]
        public DateTime Dob { get; set; }

        [Required]
        public string TrainingManagerCode { get; set; }
    }
}
