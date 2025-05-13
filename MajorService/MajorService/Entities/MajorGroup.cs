namespace MajorService.Entities
{
    public class MajorGroup : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
        public List<Major> Majors { get; set; }
    }
}
