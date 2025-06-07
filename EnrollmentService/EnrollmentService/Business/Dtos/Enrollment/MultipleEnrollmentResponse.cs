namespace EnrollmentService.Business.Dtos.Enrollment
{
    public class MultipleEnrollmentResponse
    {
        public List<EnrollmentReadDto> SuccessfulEnrollments { get; set; } = new();
        public List<EnrollmentFailure> FailedEnrollments { get; set; } = new();
        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
    }

    public class EnrollmentFailure
    {
        public Guid AcademicClassId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
