namespace EnrollmentService.Business.Dtos.Enrollment
{
    public class EnrollmentExistenceResult
    {
        public Guid AcademicClassId { get; set; }
        public bool Exists { get; set; }
    }

    public class CheckMultipleEnrollmentsResponse
    {
        public List<EnrollmentExistenceResult> Results { get; set; } = new List<EnrollmentExistenceResult>();
    }
}
