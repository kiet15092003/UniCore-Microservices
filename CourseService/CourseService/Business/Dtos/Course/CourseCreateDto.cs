namespace CourseService.Business.Dtos.Course
{
    public class CourseCreateDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public bool IsOpening { get; set; } = false;
        public int Credit { get; set; }
        public bool IsHavePracticeClass { get; set; } = false;
        public bool IsUseForCalculateScore { get; set; } = false;
        public int? MinCreditCanApply { get; set; }
        public Guid? MajorId { get; set; }
        public Guid? CompulsoryCourseId { get; set; }
        public Guid? ParallelCourseId { get; set; }
    }
}
