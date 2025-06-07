namespace EnrollmentService.Business.Dtos.Enrollment
{
    public class CheckMultipleEnrollmentsRequest
    {
        public Guid StudentId { get; set; }
        public List<Guid> AcademicClassIds { get; set; } = new List<Guid>();
    }
}
