namespace CourseService.Entities
{
    public class AcademicClass : BaseEntity
    {
        public string Name { get; set; }
        public int GroupNumber { get; set; }
        public int Capacity { get; set; }   
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int> ListOfWeeks { get; set; }
        public bool IsRegistrable { get; set; }
        public Guid SemesterId { get; set; }
        public Semester Semester { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public Guid ScheduleInDayId { get; set; }
        public ScheduleInDay ScheduleInDay { get; set; }
    }
}
