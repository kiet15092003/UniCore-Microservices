using System.ComponentModel.DataAnnotations;

namespace MajorService.Business.Dtos.Major
{
    public class MajorCreateDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Code { get; set; }
        
        [Required]
        public double CostPerCredit { get; set; }
        
        [Required]
        public Guid MajorGroupId { get; set; }
    }
}