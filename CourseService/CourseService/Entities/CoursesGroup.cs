namespace CourseService.Entities
{
    public class CoursesGroup : BaseEntity
    {
        public int SemesterNumber { get; set; }
        public List<Course> Courses { get; set; }
    }
}
