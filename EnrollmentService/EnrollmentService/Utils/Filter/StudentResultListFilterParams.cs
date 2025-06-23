namespace EnrollmentService.Utils.Filter
{
    public class StudentResultListFilterParams
    {
        public Guid? EnrollmentId { get; set; }
        public Guid? ScoreTypeId { get; set; }
        public double? MinScore { get; set; }
        public double? MaxScore { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
} 