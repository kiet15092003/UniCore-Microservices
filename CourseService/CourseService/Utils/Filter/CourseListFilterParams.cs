namespace CourseService.Utils.Filter
{
    public class CourseListFilterParams
    {
        public string Name { get; set; } = "";
        public bool? IsActive { get; set; }
        public int? MinCredit { get; set; }
        public int? MaxCredit { get; set; }
        public string? Code { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsOpenForAll { get; set; }
        public Guid[]? MajorIds { get; set; }
        public Guid[]? PreCourseIds { get; set; }
        public Guid[]? ParallelCourseIds { get; set; }
    }
}
