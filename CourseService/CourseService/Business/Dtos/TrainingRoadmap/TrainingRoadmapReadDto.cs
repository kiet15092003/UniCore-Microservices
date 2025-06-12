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
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public dynamic? MajorData { get; set; }
        public bool IsActive { get; set; }
        public List<CoursesGroupSemesterReadDto>? CoursesGroupSemesters { get; set; }
        public List<TrainingRoadmapCourseReadDto>? TrainingRoadmapCourses { get; set; }
        public List<Guid>? BatchIds { get; set; }
        public List<BatchData>? BatchDatas { get; set; }
    }
}