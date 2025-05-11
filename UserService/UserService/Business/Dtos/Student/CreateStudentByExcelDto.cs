using System.ComponentModel.DataAnnotations;

namespace UserService.Business.Dtos.Student
{
    public class CreateStudentByExcelDto 
    {
        [Required]
        public Guid BatchId { get; set; }
        [Required]
        public Guid MajorId { get; set; }
        [Required]
        public IFormFile ExcelFile { get; set; }
    }
}
