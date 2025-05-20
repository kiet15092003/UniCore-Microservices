using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace UserService.Business.Dtos.Auth
{
    public class ChangePasswordDto : IValidatableObject
    {
        [Required(ErrorMessage = "Email is required"), EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Current password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", 
            ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, and one number")]
        public string NewPassword { get; set; } = string.Empty;
        // Custom validation logic
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Check if new password is different from the old password
            if (NewPassword == Password)
            {
                yield return new ValidationResult(
                    "New password must be different from your current password.",
                    new[] { nameof(NewPassword) });
            }
            
            // Additional security checks could be added here
            if (Email.ToLower().Contains(NewPassword.ToLower()) || NewPassword.ToLower().Contains(Email.ToLower()))
            {
                yield return new ValidationResult(
                    "Password should not contain or be contained in your email address.",
                    new[] { nameof(NewPassword) });
            }
        }
    }
}
