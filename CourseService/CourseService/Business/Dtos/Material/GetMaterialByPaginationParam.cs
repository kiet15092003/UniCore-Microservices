using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Dtos.Material
{
    public class GetMaterialByPaginationParam
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public MaterialListFilterParams Filter { get; set; } = new MaterialListFilterParams();
        public Order Order { get; set; } = new Order();
    }
}
