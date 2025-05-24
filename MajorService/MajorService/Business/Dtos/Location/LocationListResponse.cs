namespace MajorService.Business.Dtos.Location
{
    public class LocationListResponse
    {
        public List<LocationReadDto> Data { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
