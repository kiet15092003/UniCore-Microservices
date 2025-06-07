namespace CourseService.Entities
{
    public class CoursesGroup : BaseEntity
    {
        public string GroupName { get; set; }
        public List<CoursesGroupSemester> CoursesGroupSemesters { get; set; }
        public List<Guid> CourseIds { get; set; }
        public Guid? MajorId { get; set; }
    }
}
