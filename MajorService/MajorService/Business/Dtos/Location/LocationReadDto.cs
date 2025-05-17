namespace MajorService.Business.Dtos.Location
{
    public class LocationReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string AddressDetail { get; set; }
        public string ImageURL { get; set; }
        public bool IsActive { get; set; }
        public int TotalBuilding { get; set; }
        public int TotalFloor { get; set; }
        public int TotalRoom { get; set; }
    }
}
