using System.ComponentModel.DataAnnotations;
using UserService.Business.Dtos.Address;

namespace UserService.Business.Dtos.Lecturer
{
    public class UpdateLecturerDto
    {
        public string? FirstName { get; set; }
        
        public string? LastName { get; set; }
        
        public string? PersonId { get; set; }
        
        [Phone]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must contain exactly 10 digits")]
        public string? PhoneNumber { get; set; }
        
        public string? Dob { get; set; }
        
        public string? Degree { get; set; }
        
        public decimal? Salary { get; set; }
        
        public Guid? DepartmentId { get; set; }
        
        public int? WorkingStatus { get; set; }
        
        public string? MainMajor { get; set; }
        
        public AddressDto? Address { get; set; }
    }
} 