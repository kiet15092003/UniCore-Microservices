namespace MajorService.Entities
{
    public class Location : BaseEntity
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }    
        public string Ward { get; set; }    
        public string AddressDetail { get; set; }   
        public string? ImageURL { get; set; }
    }
}
