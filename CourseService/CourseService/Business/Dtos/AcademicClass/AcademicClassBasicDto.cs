using CourseService.Business.Dtos.Course;
using CourseService.Business.Dtos.Semester;

namespace CourseService.Business.Dtos.AcademicClass
{
    /// <summary>
    /// Basic Academic Class DTO without circular references
    /// Used for parent/child relationships to avoid infinite recursion
    /// </summary>
    public class AcademicClassBasicDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GroupNumber { get; set; }
        public int Capacity { get; set; }
        public List<int> ListOfWeeks { get; set; } = new List<int>();
        public bool IsRegistrable { get; set; }
        public Guid CourseId { get; set; }
        public CourseReadDto? Course { get; set; }
        public Guid SemesterId { get; set; }
        public SemesterReadDto? Semester { get; set; }        
        public List<ScheduleInDayReadDto> ScheduleInDays { get; set; } = new List<ScheduleInDayReadDto>();
        public DateTime? RegistrationOpenTime { get; set; }
        public DateTime? RegistrationCloseTime { get; set; }
    }
}
