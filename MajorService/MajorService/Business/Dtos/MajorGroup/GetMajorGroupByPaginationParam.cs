using MajorService.Utils.Filter;
using MajorService.Utils.Pagination;

namespace MajorService.Business.Dtos.MajorGroup
{
    public class GetMajorGroupByPaginationParam
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public MajorGroupListFilterParams Filter { get; set; } = new MajorGroupListFilterParams();
        public Order Order { get; set; } = new Order();
    }
}
