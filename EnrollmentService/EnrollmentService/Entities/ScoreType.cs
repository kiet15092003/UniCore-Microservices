namespace EnrollmentService.Entities
{
    public class ScoreType : BaseEntity
    {
        public int Type { get; set; }
        public int Percentage { get; set; }
        public List<StudentResult> StudentResults { get; set; }
    }
}
