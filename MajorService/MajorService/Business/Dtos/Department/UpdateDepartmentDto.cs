using System.ComponentModel.DataAnnotations;

namespace MajorService.Business.Dtos.Department
{
    public class UpdateDepartmentDto
    {
        [Required]
        public string Name { get; set; }
    }
} 