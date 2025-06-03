using EnrollmentService.Utils.Pagination;

namespace EnrollmentService.Business.Dtos.Enrollment
{
    public class EnrollmentListResponse
    {
        public List<EnrollmentReadDto> Data { get; set; } = new();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; } = 0;
        public int TotalPages { get; set; } = 1;

        public static EnrollmentListResponse FromPaginationResult(PaginationResult<EnrollmentReadDto> paginationResult)
        {
            return new EnrollmentListResponse
            {
                Data = paginationResult.Items,
                Page = paginationResult.Page,
                PageSize = paginationResult.PageSize,
                TotalCount = paginationResult.TotalCount,
                TotalPages = paginationResult.TotalPages
            };
        }
    }
}
