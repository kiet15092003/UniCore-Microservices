namespace CourseService.Business.Dtos.Material
{
    public class MaterialTypeUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
} 