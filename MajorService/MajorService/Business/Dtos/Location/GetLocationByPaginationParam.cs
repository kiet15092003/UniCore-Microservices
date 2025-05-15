using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.Business.Dtos.Location
{
    public class GetLocationByPaginationParam
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public LocationListFilterParams Filter { get; set; } = new LocationListFilterParams();
        public Order? Order { get; set; }
    }
}
