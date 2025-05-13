using CourseService.Business.Dtos.Course;

namespace CourseService.Business.Dtos.TrainingRoadmap
{
    public class TrainingRoadmapCourseReadDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public int SemesterNumber { get; set; }
        public CourseReadDto? Course { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
