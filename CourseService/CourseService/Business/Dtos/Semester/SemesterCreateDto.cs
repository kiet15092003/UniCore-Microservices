using System.ComponentModel.DataAnnotations;

namespace CourseService.Business.Dtos.Semester
{
    public class SemesterCreateDto
    {
        [Required]
        [Range(1, 3, ErrorMessage = "SemesterNumber must be between 1 and 3")]
        public int SemesterNumber { get; set; }
        
        [Required]
        [Range(2020, 2050, ErrorMessage = "Year must be between 2020 and 2050")]
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfWeeks { get; set; }
    }
}
