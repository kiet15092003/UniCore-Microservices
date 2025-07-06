namespace EnrollmentService.Business.Dtos.Enrollment
{
    public class EnrollmentScoreDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid AcademicClassId { get; set; }
        public string? AcademicClassName { get; set; }
        public string? CourseName { get; set; }
        public string? CourseCode { get; set; }
        public int TotalCredits { get; set; }
        public double? OverallScore { get; set; }
        public bool IsPassed { get; set; }
        public List<ComponentScoreDto> ComponentScores { get; set; } = new List<ComponentScoreDto>();
        public List<PracticeScoreDto> PracticeScores { get; set; } = new List<PracticeScoreDto>();
    }

    public class ComponentScoreDto
    {
        public Guid ScoreTypeId { get; set; }
        public int ScoreType { get; set; }
        public double Score { get; set; }
        public int Percentage { get; set; }
    }

    public class PracticeScoreDto
    {
        public Guid PracticeClassId { get; set; }
        public string PracticeClassName { get; set; } = string.Empty;
        public List<ComponentScoreDto> ComponentScores { get; set; } = new List<ComponentScoreDto>();
    }
} 