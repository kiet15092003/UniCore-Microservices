namespace EnrollmentService.Business.Dtos.Exam
{
    public class EnrollmentExamDto
    {
        public Guid Id { get; set; }
        public Guid ExamId { get; set; }
        public Guid EnrollmentId { get; set; }
        public int Status { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
