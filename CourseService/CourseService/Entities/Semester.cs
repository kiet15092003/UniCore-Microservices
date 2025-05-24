namespace CourseService.Entities
{
    public class Semester : BaseEntity
    {
        public int SemesterNumber { get; set; }
        public int Year { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
