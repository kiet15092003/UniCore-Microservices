using System.ComponentModel.DataAnnotations;

namespace StudentService.Business.Dtos.Batch
{
    public class BatchUpdateDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public int StartYear { get; set; }
    }
} 