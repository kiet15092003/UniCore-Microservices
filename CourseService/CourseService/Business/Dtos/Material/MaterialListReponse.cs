using CourseService.Business.Dtos.Material;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Dtos.Material
{
    public class MaterialListResponse : PaginationResult<MaterialReadDto>
    {
        public List<MaterialReadDto> Data { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
