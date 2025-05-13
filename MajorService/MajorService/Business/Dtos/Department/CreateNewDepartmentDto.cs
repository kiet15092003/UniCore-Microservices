using System.ComponentModel.DataAnnotations;

namespace MajorService.Business.Dtos.Department
{
    public class CreateNewDepartmentDto
    {
        [Required]
        public string Name { get; set; }
    }
}
