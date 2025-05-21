namespace UserService.Entities
{
    public class Guardian : BaseEntity
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Relationship { get; set; }
        public Guid? StudentId { get; set; }
        public Student Student { get; set; }
    }
}
