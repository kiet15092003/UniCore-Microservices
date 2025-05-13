namespace CourseService.Business.Dtos.Course
{
    public class CourseCreateDto
    {
        // Code is completely removed as it will be auto-generated in the repository
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsRegistrable { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public int Credit { get; set; }
        public int PracticePeriod { get; set; } = 0;
        public bool IsRequired { get; set; } = false;
        public int? MinCreditRequired { get; set; }
        public Guid MajorId { get; set; }
        public Guid[]? PreCourseIds { get; set; }
        public Guid[]? ParallelCourseIds { get; set; }
    }
}
