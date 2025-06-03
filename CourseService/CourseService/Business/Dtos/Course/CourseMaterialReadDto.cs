namespace CourseService.Business.Dtos.Course
{
    public class CourseMaterialReadDto
    {
        public Guid MaterialId { get; set; }
        public string Name { get; set; }
        public string FileUrl { get; set; }
        public Guid? MaterialTypeId { get; set; }
        public string MaterialTypeName { get; set; }
    }
}
