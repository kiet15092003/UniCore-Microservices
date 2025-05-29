using CourseService.Business.Dtos.Course;
using CourseService.Business.Dtos.Semester;

namespace CourseService.Business.Dtos.AcademicClass
{
    public class ScheduleInDayCreateDto
    {
        public string DayOfWeek { get; set; }
        public Guid RoomId { get; set; }
        public Guid AcademicClassId { get; set; }
        public Guid ShiftId { get; set; }
    }

    public class ScheduleInDayCreateForClassDto
    {
        public string DayOfWeek { get; set; }
        public Guid RoomId { get; set; }
        public Guid ShiftId { get; set; }
    }
    
    public class AcademicClassCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public int GroupNumber { get; set; }
        public int Capacity { get; set; }
        public List<int> ListOfWeeks { get; set; } = new List<int>();
        public bool IsRegistrable { get; set; }
        public Guid CourseId { get; set; }
        public Guid SemesterId { get; set; }
        public List<ScheduleInDayCreateForClassDto> ScheduleInDays { get; set; } = new List<ScheduleInDayCreateForClassDto>();
    }
}
