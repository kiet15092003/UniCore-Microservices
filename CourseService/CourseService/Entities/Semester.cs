namespace CourseService.Entities
{
    public class Semester : BaseEntity
    {
        public int SemesterNumber { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfWeeks { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
