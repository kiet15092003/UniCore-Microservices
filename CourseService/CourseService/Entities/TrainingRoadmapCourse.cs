namespace CourseService.Entities
{
    public class TrainingRoadmapCourse : BaseEntity
    {
        public Guid TrainingRoadmapId { get; set; }
        public TrainingRoadmap TrainingRoadmap { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public int SemesterNumber { get; set; }
    }
}
