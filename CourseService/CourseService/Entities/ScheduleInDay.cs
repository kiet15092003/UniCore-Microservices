namespace CourseService.Entities
{
    public class ScheduleInDay : BaseEntity
    {
        public string DayOfWeek { get; set; }
        public Guid RoomId { get; set; }
        public Guid AcademicClassId { get; set; }
        public AcademicClass AcademicClass { get; set; }
        public Guid ShiftId { get; set; }
        public Shift Shift { get; set; }
    }
}
