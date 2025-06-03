using System.ComponentModel.DataAnnotations;

namespace CourseService.Business.Dtos.AcademicClass
{    
    public class ClassRegistrationScheduleDto
    {
        [Required]
        public List<Guid> AcademicClassIds { get; set; }
        
        [Required]
        public DateTime RegistrationOpenTime { get; set; }
        
        [Required]
        public DateTime RegistrationCloseTime { get; set; }
    }
}
