namespace CourseService.Business.Dtos.Semester
{
    public class SemesterFilterParams
    {
        public int? Year { get; set; }
        public int? SemesterNumber { get; set; }
        public bool? IsActive { get; set; }
    }
}
