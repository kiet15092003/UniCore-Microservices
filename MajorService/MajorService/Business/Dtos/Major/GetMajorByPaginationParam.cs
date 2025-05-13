using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.Business.Dtos.Major
{
    public class GetMajorByPaginationParam
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public MajorListFilterParams Filter { get; set; } = new MajorListFilterParams();
        public Order Order { get; set; } = new Order();
    }
}
