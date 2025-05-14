using System.Text.Json.Serialization;
namespace UserService.Entities
{
    public class Batch : BaseEntity
    {
        public string Title { get; set; }
        public int StartYear { get; set; }
        public List<Student> Students { get; set; }
    }
}
