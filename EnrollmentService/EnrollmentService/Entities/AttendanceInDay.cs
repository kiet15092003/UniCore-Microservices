namespace EnrollmentService.Entities
{
    public class AttendanceInDay : BaseEntity
    {
        public Guid StudentResultId { get; set; }
        public StudentResult StudentResult { get; set; }
        public DateOnly Date { get; set; }
    }
}
