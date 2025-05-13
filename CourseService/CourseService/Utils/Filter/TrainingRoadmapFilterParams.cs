namespace CourseService.Utils.Filter
{
    public class TrainingRoadmapFilterParams
    {
        public string SearchQuery { get; set; } = "";
        public int? StartYear { get; set; }
        public string? Code { get; set; }
        public Guid? MajorId { get; set; }
    }
}