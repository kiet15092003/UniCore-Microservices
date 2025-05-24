using System.ComponentModel.DataAnnotations;

namespace CourseService.Business.Dtos.CoursesGroup
{
    public class CoursesGroupCreateDto
    {
        [Required]
        public string GroupName { get; set; }     
        [Required]
        public Guid MajorId { get; set; }
        public int Credit { get; set; }
        public List<Guid> CourseIds { get; set; } = new List<Guid>();
    }
}