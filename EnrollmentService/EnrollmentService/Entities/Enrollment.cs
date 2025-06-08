namespace EnrollmentService.Entities
{
    public class Enrollment : BaseEntity
    {
        public int Status { get; set; }
        public Guid StudentId { get; set; }
        public Guid AcademicClassId { get; set; }
        public List<StudentResult> StudentResults { get; set; }
    }
}
