namespace EnrollmentService.Entities
{
    public class StudentResult : BaseEntity
    {
        public double Score { get; set; }
        public Guid EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; }
        public List<AttendanceInDay> AttendanceInDays { get; set; }
    }
}
