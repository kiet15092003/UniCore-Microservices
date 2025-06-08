using UserService.Utils.Filter;
using UserService.Utils.Pagination;

namespace UserService.Business.Dtos.Lecturer
{
    public class GetLecturerByPaginationParam
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public LecturerListFilterParams Filter { get; set; } = new LecturerListFilterParams();
        public Order Order { get; set; } = new Order();
    }
} 