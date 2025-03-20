using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string FullName { get; set; }
    [Required]
    public DateTime Dob { get; set; }
    [Required]
    public int Status { get; set; } = 1;
}
