using System.ComponentModel.DataAnnotations;
using UserService.Business.Dtos.Address;

namespace UserService.Business.Dtos.Lecturer
{
    public class LecturerDetailDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string LecturerCode { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string PersonId { get; set; }
        public int Status { get; set; }
        public string ImageUrl { get; set; }
        public string Degree { get; set; }
        public decimal Salary { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int WorkingStatus { get; set; }
        public DateTime JoinDate { get; set; }
        public string MainMajor { get; set; }
        public AddressDto Address { get; set; }
    }
} 