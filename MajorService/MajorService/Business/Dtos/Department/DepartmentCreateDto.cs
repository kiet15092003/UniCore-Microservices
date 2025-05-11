using System.ComponentModel.DataAnnotations;

namespace MajorService.Business.Dtos.Department
{
    public class DepartmentCreateDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Code { get; set; }
    }
}