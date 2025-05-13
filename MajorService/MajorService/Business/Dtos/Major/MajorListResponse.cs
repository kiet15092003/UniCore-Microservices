namespace MajorService.Business.Dtos.Major
{
    public class MajorListResponse
    {
        public List<MajorReadDto> Data { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
