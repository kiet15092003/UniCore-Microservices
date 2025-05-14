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
    public DateTime Dob { get; set; }
    [Required]
    public int Status { get; set; } = 1;
    public string ImageUrl { get; set; } = "";
    public Guid? AddressId { get; set; }
    public Address? Address { get; set; }
    public Student? Student { get; set; }
}
