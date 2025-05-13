namespace CourseService.Business.Dtos.TrainingRoadmap
{
    public class CoursesGroupSemesterReadDto
    {
        public Guid Id { get; set; }
        public int SemesterNumber { get; set; }
        public Guid CoursesGroupId { get; set; }
        public string? CoursesGroupName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
