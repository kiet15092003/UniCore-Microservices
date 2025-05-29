namespace CourseService.Entities
{
    public class Material : BaseEntity
    {
        public string Name { get; set; }
        public string FileUrl { get; set; }
        public Guid? MaterialTypeId { get; set; }
        public MaterialType MaterialType { get; set; }
        public List<CourseMaterial> CourseMaterials { get; set; }
    }
}
