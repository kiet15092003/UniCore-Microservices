namespace CourseService.Entities
{
    public class Shift : BaseEntity
    {
        public string Name { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
