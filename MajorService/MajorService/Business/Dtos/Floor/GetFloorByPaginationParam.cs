using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.Business.Dtos.Floor
{
    public class GetFloorByPaginationParam
    {
        public Pagination Pagination { get; set; } = new();
        public FloorListFilterParams Filter { get; set; } = new();
        public Order? Order { get; set; }
    }
}
