namespace EnrollmentService.Entities
{
    public class EnrollmentExam : BaseEntity
    {
        public Guid ExamId { get; set; }
        public Guid EnrollmentId { get; set; }    
        public Exam? Exam { get; set; }
        public Enrollment? Enrollment { get; set; }
    }
}
