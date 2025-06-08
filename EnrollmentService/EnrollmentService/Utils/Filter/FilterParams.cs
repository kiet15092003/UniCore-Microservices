namespace EnrollmentService.Utils.Filter
{
    public class Order
    {
        public string OrderBy { get; set; } = "createdAt";
        public string Direction { get; set; } = "desc";
    }

    public class EnrollmentListFilterParams
    {
        public Guid? StudentId { get; set; }
        public Guid? AcademicClassId { get; set; }
        public int? Status { get; set; }
    }
}
