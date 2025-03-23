namespace CourseService.Entities
{
    public class Material : BaseEntity
    {
        public string Name { get; set; }
        public string FileUrl { get; set; }
        public List<CourseMaterial> CourseMaterials { get; set; }
    }
}
