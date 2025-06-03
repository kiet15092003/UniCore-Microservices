namespace CourseService.Utils.Filter
{
    public class MaterialListFilterParams
    {
        public string? Name { get; set; }
        public Guid? MaterialTypeId { get; set; }
        public bool? HasFile { get; set; }
    }
}

