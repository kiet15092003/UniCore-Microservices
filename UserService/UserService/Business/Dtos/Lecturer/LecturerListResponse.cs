using System.Collections.Generic;
using UserService.Utils.Pagination;

namespace UserService.Business.Dtos.Lecturer
{
    public class LecturerListResponse
    {
        public List<LecturerDto> Data { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
} 