using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using UserService.Entities;
using System.Text.Json.Serialization;
public class ApplicationUser : IdentityUser
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string PersonId { get; set; }
    
    [Required]
    [Phone]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must contain exactly 10 digits")]
    public override string PhoneNumber { get; set; }
    [Required]
    public DateTime Dob { get; set; }
    [Required]
    public int Status { get; set; } = 1;
    public string ImageUrl { get; set; } = "";
    public Guid? AddressId { get; set; }
    public Address? Address { get; set; }
    public Student? Student { get; set; }
}
