namespace MajorService.Business.Dtos.Location
{
    public class UpdateLocationDto
    {
        public required string Name { get; set; }
        public required string Country { get; set; }
        public required string City { get; set; }
        public required string District { get; set; }
        public required string Ward { get; set; }
        public required string AddressDetail { get; set; }
        public string? ImageURL { get; set; }
    }
}
