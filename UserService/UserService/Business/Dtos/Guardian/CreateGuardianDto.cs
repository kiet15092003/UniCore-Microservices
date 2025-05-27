namespace UserService.Business.Dtos.Guardian
{
    public class CreateGuardianDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Relationship { get; set; }
        public Guid StudentId { get; set; }
    }
}
