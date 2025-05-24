using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.Business.Dtos.Building
{
    public class GetBuildingByPaginationParam
    {
        public Pagination Pagination { get; set; } = new();
        public BuildingListFilterParams Filter { get; set; } = new();
        public Order? Order { get; set; }
    }
}
