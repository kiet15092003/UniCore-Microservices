using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UserService.Business.Dtos.Lecturer
{
    public class CreateLecturerByExcelDto
    {
        [Required]
        public IFormFile ExcelFile { get; set; }
        
        [Required]
        public Guid DepartmentId { get; set; }
        
        [Required]
        public int WorkingStatus { get; set; }
    }
} 