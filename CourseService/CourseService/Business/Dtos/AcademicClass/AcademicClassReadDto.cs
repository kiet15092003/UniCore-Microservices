using CourseService.Business.Dtos.Course;
using CourseService.Business.Dtos.Semester;
using CourseService.Business.Dtos.Shift;
using MajorService;

namespace CourseService.Business.Dtos.AcademicClass
{
    public class ScheduleInDayReadDto
    {
        public Guid Id { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public Guid RoomId { get; set; }
        public RoomData? Room { get; set; }
        public Guid ShiftId { get; set; }
        public ShiftDto? Shift { get; set; }
    }    
    public class AcademicClassReadDto
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
        public Guid? ParentTheoryAcademicClassId { get; set; }
        public AcademicClassBasicDto? ParentTheoryAcademicClass { get; set; }
        public List<Guid> ChildPracticeAcademicClassIds { get; set; } = new List<Guid>();
        public List<AcademicClassBasicDto> ChildPracticeAcademicClasses { get; set; } = new List<AcademicClassBasicDto>();
        public List<ScheduleInDayReadDto> ScheduleInDays { get; set; } = new List<ScheduleInDayReadDto>();
        public DateTime? RegistrationOpenTime { get; set; }
        public DateTime? RegistrationCloseTime { get; set; }
        public int EnrollmentCount { get; set; }
    }
}
