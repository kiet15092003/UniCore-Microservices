namespace MajorService.Business.Dtos.Location
{
    public class CreateNewLocationDto
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string AddressDetail { get; set; }
        public string ImageURL { get; set; }
    }
}
