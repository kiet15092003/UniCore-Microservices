namespace UserService.Utils.Filter
{
    public class StudentListFilterParams
    {
        public string SearchQuery { get; set; } = "";
        public Guid? MajorId { get; set; }
        public Guid? BatchId { get; set; }
    }
}
