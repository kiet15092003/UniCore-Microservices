namespace CourseService.Entities
{
    public class TrainingRoadmap : BaseEntity
    {
        public Guid MajorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int StartYear { get; set; }
        public List<CoursesGroupSemester> CoursesGroupSemesters { get; set; }
        public List<TrainingRoadmapCourse> TrainingRoadmapCourses { get; set; }
        public bool IsActive { get; set; } = true;
        public List<Guid> BatchIds { get; set; }
    }
}
