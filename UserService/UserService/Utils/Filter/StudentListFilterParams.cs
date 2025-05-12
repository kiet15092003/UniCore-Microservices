namespace UserService.Utils.Filter
{
    public class StudentListFilterParams
    {
        public string SearchQuery { get; set; } = "";
        public bool? IsActive { get; set; }
        public int? MinCredit { get; set; }
        public int? MaxCredit { get; set; }
        public string? Code { get; set; }
        public bool? IsRegistrable { get; set; }
        public bool? IsRequired { get; set; }
        public Guid? MajorId { get; set; }
        public Guid[]? PreCourseIds { get; set; }
        public Guid[]? ParallelCourseIds { get; set; }
    }
}
