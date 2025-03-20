namespace MajorService.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public List<MajorGroup> MajorGroups { get; set; }   
    }
}
