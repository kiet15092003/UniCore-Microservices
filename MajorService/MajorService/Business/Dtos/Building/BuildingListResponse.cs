using System.Collections.Generic;

namespace MajorService.Business.Dtos.Building
{
    public class BuildingListResponse
    {
        public List<BuildingReadDto> Data { get; set; } = new();
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
