using System.ComponentModel.DataAnnotations;

namespace MajorService.Business.Dtos.MajorGroup
{
    public class CreateNewMajorGroupDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public Guid DepartmentId { get; set; }
    }
}
