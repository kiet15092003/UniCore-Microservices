namespace EnrollmentService.Utils.Filter
{    public class Order
    {
        public string OrderBy { get; set; } = "EnrolledDate";
        public bool IsDesc { get; set; } = false;
    }
    public class EnrollmentListFilterParams
    {
        public string? Search { get; set; }
        public string? StudentCode { get; set; }
        public Guid? AcademicClassId { get; set; }
        public int? Status { get; set; }
        public Guid? SemesterId { get; set; }
        public Guid? CourseId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? LecturerName { get; set; }
        public string? ClassName { get; set; }
    }
}
