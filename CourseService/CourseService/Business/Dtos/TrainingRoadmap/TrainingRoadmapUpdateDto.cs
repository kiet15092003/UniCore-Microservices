namespace CourseService.Business.Dtos.TrainingRoadmap
{
    public class TrainingRoadmapUpdateDto
    {
        public Guid? MajorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StartYear { get; set; }
        public List<Guid>? BatchIds { get; set; }
    }
}