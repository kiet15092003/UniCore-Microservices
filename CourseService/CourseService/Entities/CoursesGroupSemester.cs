namespace CourseService.Entities
{
    public class CoursesGroupSemester : BaseEntity
    {
        public int SemesterNumber { get; set; }

        public Guid CoursesGroupId { get; set; }
        public CoursesGroup CoursesGroup { get; set; }

        public Guid TrainingRoadmapId { get; set; }
        public TrainingRoadmap TrainingRoadmap { get; set; }
    }
}
