namespace CourseService.Utils.Filter
{
    public class SemesterFilterParams
    {
        public int? Year { get; set; }
        public int? SemesterNumber { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? NumberOfWeeks { get; set; }
    }
}
