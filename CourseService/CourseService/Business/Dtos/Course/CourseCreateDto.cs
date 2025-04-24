namespace CourseService.Business.Dtos.Course
{
    public class CourseCreateDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }
        public bool IsRegistrable { get; set; } = false;
        public int Credit { get; set; }
        public int PracticePeriod { get; set; } = 0;
        public bool IsRequired { get; set; } = false;
        public int? MinCreditRequired { get; set; }
        public Guid MajorId { get; set; }
        public Guid[]? PreCourseIds { get; set; }
        public Guid[]? ParallelCourseIds { get; set; }
    }
}
