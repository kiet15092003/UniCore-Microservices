using System.ComponentModel.DataAnnotations;

namespace CourseService.Entities
{
    public class TrainingManager : BaseEntity
    {
        public string TrainingManagerCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
