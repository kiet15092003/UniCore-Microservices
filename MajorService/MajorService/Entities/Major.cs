namespace MajorService.Entities
{
    public class Major : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public Guid MajorGroupId { get; set; }
        public MajorGroup MajorGroup { get; set; }
    }
}
