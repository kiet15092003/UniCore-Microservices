namespace EnrollmentService.Business.Dtos.Enrollment
{
    public class EnrollmentConflictCheckDto : EnrollmentReadDto
    {
        public bool IsConflict { get; set; }
    }
    public class CheckClassConflictRequest
    {
        public Guid? ClassToCheckId { get; set; }
        public List<Guid> EnrollmentIds { get; set; } = new List<Guid>();
    }

    public class CheckClassConflictResponse
    {
        public List<EnrollmentConflictCheckDto> Enrollments { get; set; } = new List<EnrollmentConflictCheckDto>();
    }
}
