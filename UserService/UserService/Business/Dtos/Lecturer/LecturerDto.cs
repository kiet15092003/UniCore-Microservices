using System.ComponentModel.DataAnnotations;
using UserService.Business.Dtos.Student;

namespace UserService.Business.Dtos.Lecturer
{
    public class LecturerDto
    {
        public Guid Id { get; set; }
        
        [Required]
        public string LecturerCode { get; set; }
        
        [Required]
        public string Degree { get; set; }
        
        [Required]
        public decimal Salary { get; set; }
        
        [Required]
        public Guid DepartmentId { get; set; }
        
        [Required]
        public int WorkingStatus { get; set; }
        
        [Required]
        public DateTime JoinDate { get; set; }
        
        public string MainMajor { get; set; }

        public ApplicationUserDto ApplicationUser { get; set; }
    }
} 