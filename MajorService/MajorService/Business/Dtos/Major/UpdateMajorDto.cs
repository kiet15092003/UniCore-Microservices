using System.ComponentModel.DataAnnotations;

namespace MajorService.Business.Dtos.Major
{
    public class UpdateMajorDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public double CostPerCredit { get; set; }
        
        public Guid? MajorGroupId { get; set; }
    }
} 