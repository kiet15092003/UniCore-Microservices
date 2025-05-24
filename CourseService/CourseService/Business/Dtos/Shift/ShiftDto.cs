namespace CourseService.Business.Dtos.Shift
{
    public class ShiftDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
