using CourseService.Business.Dtos.Course;
using CourseService.Entities;

namespace CourseService.Business.Dtos.TrainingRoadmap
{
    public class TrainingRoadmapReadDto
    {
        public Guid Id { get; set; }
        public Guid? MajorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int StartYear { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public dynamic? MajorData { get; set; }
        public List<CoursesGroup> CoursesGroups { get; set; }
        public List<TrainingRoadmapCourse> TrainingRoadmapCourses { get; set; }
    }
}