using System;
using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.Business.Dtos.Room
{
    public class GetRoomByPaginationDto
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public RoomListFilterParams Filter { get; set; } = new RoomListFilterParams();
        public Order? Order { get; set; }
    }
}
