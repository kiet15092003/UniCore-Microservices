using System.Collections.Generic;

namespace MajorService.Business.Dtos.MajorGroup
{
    public class MajorGroupListResponse
    {
        public List<MajorGroupReadDto> Data { get; set; } = new List<MajorGroupReadDto>();
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
