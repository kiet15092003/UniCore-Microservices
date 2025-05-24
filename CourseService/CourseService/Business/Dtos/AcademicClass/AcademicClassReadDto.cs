using CourseService.Business.Dtos.Course;
using CourseService.Business.Dtos.Semester;
using CourseService.Business.Dtos.Shift;

namespace CourseService.Business.Dtos.AcademicClass
{
    public class ScheduleInDayReadDto
    {
        public Guid Id { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public Guid RoomId { get; set; }
        public Guid ShiftId { get; set; }
        public ShiftDto? Shift { get; set; }
    }

    public class AcademicClassReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GroupNumber { get; set; }
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int> ListOfWeeks { get; set; } = new List<int>();
        public bool IsRegistrable { get; set; }
        public Guid CourseId { get; set; }
        public CourseReadDto? Course { get; set; }
        public Guid SemesterId { get; set; }
        public SemesterReadDto? Semester { get; set; }
        public Guid ScheduleInDayId { get; set; }
        public ScheduleInDayReadDto? ScheduleInDay { get; set; }
    }
}
