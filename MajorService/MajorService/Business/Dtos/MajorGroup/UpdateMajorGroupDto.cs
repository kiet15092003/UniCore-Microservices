using System.ComponentModel.DataAnnotations;

namespace MajorService.Business.Dtos.MajorGroup
{
    public class UpdateMajorGroupDto
    {
        [Required]
        public string Name { get; set; }
        
        public Guid? DepartmentId { get; set; }
    }
}