namespace MajorService.Business.Dtos.Major
{
    public class MajorListFilterParams
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public bool? IsActive { get; set; }
        public Guid? MajorGroupId { get; set; }
    }
}
