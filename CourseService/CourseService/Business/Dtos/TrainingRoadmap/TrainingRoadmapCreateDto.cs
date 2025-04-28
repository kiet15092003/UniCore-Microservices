namespace CourseService.Business.Dtos.TrainingRoadmap
{
    public class TrainingRoadmapCreateDto
    {
        public Guid? MajorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StartYear { get; set; }
        // Code will be generated automatically
    }
}