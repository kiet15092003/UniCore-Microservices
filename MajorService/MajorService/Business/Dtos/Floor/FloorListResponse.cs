using System.Collections.Generic;

namespace MajorService.Business.Dtos.Floor
{
    public class FloorListResponse
    {
        public List<FloorReadDto> Data { get; set; } = new();
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
