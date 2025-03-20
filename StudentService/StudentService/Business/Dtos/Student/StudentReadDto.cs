using StudentService.Entities;
using System.ComponentModel.DataAnnotations;

namespace StudentService.Business.Dtos.Student
{
    public class StudentReadDto
    {
        public Guid Id { get; set; }
        public string StudentCode { get; set; }
        public int TotalCredits { get; set; }
        public Guid MajorId { get; set; }
        public Guid BatchId { get; set; }
        public Batch Batch { get; set; }
        public Guid ApplicationUserId { get; set; }
    }
}
