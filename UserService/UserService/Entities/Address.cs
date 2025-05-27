namespace UserService.Entities
{
    public class Address : BaseEntity
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string AddressDetail { get; set; }   
        public ApplicationUser ApplicationUser { get; set; }
    }
}
