using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;

namespace EnrollmentService.Business.Dtos.Exam
{
    public class GetExamByPaginationParam
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public ExamListFilterParams Filter { get; set; } = new ExamListFilterParams();
        public Order? Order { get; set; }
    }

    public class ExamListResponse
    {
        public List<ExamReadDto> Data { get; set; } = new List<ExamReadDto>();
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
