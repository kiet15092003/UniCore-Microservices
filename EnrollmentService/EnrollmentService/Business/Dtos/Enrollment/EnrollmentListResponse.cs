using EnrollmentService.Utils.Pagination;

namespace EnrollmentService.Business.Dtos.Enrollment
{
    public class EnrollmentListResponse
    {
        public List<EnrollmentReadDto> Data { get; set; } = new();
        public int Total { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 0;

        public static EnrollmentListResponse FromPaginationResult(PaginationResult<EnrollmentReadDto> paginationResult)
        {
            return new EnrollmentListResponse
            {
                Data = paginationResult.Data,
                Total = paginationResult.Total,
                PageSize = paginationResult.PageSize,
                PageIndex = paginationResult.PageIndex,
            };
        }
    }
}
