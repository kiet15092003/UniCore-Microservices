namespace CourseService.Entities
{
    public class Course : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int Credit { get; set; } 
        public int PracticePeriod { get; set; }
        public int TheoryPeriod { get; set; }
        public bool IsRequired { get; set; }
        public bool IsOpenForAll { get; set; } = false;
        public int? MinCreditRequired { get; set; }
        public Guid[] MajorIds { get; set; }
        public Guid[] PreCourseIds { get; set; }
        public Guid[] ParallelCourseIds { get; set; }

        public List<CourseCertificate> CourseCertificates { get; set; }
        public List<CourseMaterial> CourseMaterials { get; set; }
        public List<TrainingRoadmapCourse> TrainingRoadmapCourses { get; set; }
    }
}
