using System.ComponentModel.DataAnnotations;
using UserService.Business.Dtos.Address;

namespace UserService.Business.Dtos.Lecturer
{
    public class UpdateLecturerDto
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
        public string PhoneNumber { get; set; }
        
        [Required]
        public string Dob { get; set; }
        
        [Required]
        public string Degree { get; set; }
        
        [Required]
        public decimal Salary { get; set; }
        
        [Required]
        public Guid DepartmentId { get; set; }
        
        [Required]
        public int WorkingStatus { get; set; }
        
        public string MainMajor { get; set; }
        
        public AddressDto Address { get; set; }
    }
} 