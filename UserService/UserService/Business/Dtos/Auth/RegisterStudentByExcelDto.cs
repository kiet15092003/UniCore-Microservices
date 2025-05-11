using System.ComponentModel.DataAnnotations;

namespace UserService.Business.Dtos.Auth
{
    public class RegisterStudentByExcelDto 
    {
        [Required]
        public Guid BatchId { get; set; }
        [Required]
        public Guid MajorId { get; set; }
        [Required]
        public IFormFile ExcelFile { get; set; }
    }
}
