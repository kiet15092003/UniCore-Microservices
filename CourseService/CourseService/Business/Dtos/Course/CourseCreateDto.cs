namespace CourseService.Business.Dtos.Course
{
    public class CourseCreateDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int Credit { get; set; }
        public int PracticePeriod { get; set; } = 0;
        public bool IsRequired { get; set; } = false;
        public int? MinCreditRequired { get; set; }
        public bool IsOpenForAll { get; set; } = false;
        public Guid[] MajorIds { get; set; }
        public Guid[]? PreCourseIds { get; set; }
        public Guid[]? ParallelCourseIds { get; set; }
    }
}
