namespace CourseService.Entities
{
    public class MaterialType : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public List<Material> Materials { get; set; }
    }
} 