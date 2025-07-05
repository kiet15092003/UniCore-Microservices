namespace EnrollmentService.Entities
{
    public class EnrollmentExam : BaseEntity
    {
        public int Status { get; set; }
        public int Score { get; set; }
        public Guid ExamId { get; set; }
        public Guid EnrollmentId { get; set; }
        
        // Navigation properties
        public Exam? Exam { get; set; }
        public Enrollment? Enrollment { get; set; }
    }
}
