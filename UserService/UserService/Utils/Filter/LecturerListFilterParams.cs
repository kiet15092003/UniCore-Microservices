namespace UserService.Utils.Filter
{
    public class LecturerListFilterParams
    {
        public string SearchQuery { get; set; } = "";
        public Guid? DepartmentId { get; set; }
        public int? WorkingStatus { get; set; }
        public string? MainMajor { get; set; }
        public string? Degree { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? JoinDate { get; set; }
    }
}
