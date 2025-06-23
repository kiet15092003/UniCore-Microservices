namespace EnrollmentService.DTOs.StudentResult
{
    public class StudentResultDto
    {
        public Guid Id { get; set; }
        public double Score { get; set; }
        public Guid EnrollmentId { get; set; }
        public Guid ScoreTypeId { get; set; }
        public string ScoreTypeName { get; set; }
        public int ScoreTypePercentage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
} 