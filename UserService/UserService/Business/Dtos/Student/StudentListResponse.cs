namespace UserService.Business.Dtos.Student
{
    public class StudentListResponse
    {
        public List<StudentDto> Data { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
