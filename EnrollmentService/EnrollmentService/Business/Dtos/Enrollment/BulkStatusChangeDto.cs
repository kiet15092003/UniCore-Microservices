namespace EnrollmentService.Business.Dtos.Enrollment
{
    public class BulkStatusChangeDto
    {
        public List<Guid> ClassIds { get; set; } = new List<Guid>();
        public int Status { get; set; }
    }
}