using System.Collections.Generic;

namespace MajorService.Business.Dtos.Department
{
    public class DepartmentListResponse
    {
        public List<DepartmentReadDto> Data { get; set; } = new List<DepartmentReadDto>();
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
