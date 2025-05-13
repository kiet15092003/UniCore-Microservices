using System.ComponentModel.DataAnnotations;

namespace MajorService.Business.Dtos.Major
{
    public class CreateNewMajorDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public double CostPerCredit { get; set; }
        
        [Required]
        public Guid MajorGroupId { get; set; }
    }
}
