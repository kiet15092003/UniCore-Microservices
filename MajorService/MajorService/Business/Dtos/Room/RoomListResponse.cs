using System.Collections.Generic;

namespace MajorService.Business.Dtos.Room
{
    public class RoomListResponse
    {
        public List<RoomReadDto> Data { get; set; } = new();
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
