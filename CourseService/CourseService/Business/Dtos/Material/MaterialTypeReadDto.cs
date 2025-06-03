namespace CourseService.Business.Dtos.Material
{
    public class MaterialTypeReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid MaterialId { get; set; }
        
    }
} 