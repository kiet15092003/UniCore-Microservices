namespace UserService.Entities
{
    public class Guardian : BaseEntity
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Relationship { get; set; }
        public List<Student> Students { get; set; }
    }
}
