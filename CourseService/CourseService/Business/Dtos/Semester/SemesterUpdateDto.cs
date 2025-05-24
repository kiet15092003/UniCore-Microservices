using System.ComponentModel.DataAnnotations;

namespace CourseService.Business.Dtos.Semester
{
    public class SemesterUpdateDto
    {
        [Range(1, 3, ErrorMessage = "SemesterNumber must be between 1 and 3")]
        public int SemesterNumber { get; set; }
        
        [Range(2020, 2050, ErrorMessage = "Year must be between 2020 and 2050")]
        public int Year { get; set; }
        
        public bool IsActive { get; set; }
    }
}
