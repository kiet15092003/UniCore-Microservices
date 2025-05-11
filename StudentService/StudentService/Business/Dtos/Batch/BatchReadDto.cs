namespace StudentService.Business.Dtos.Batch
{
    public class BatchReadDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int StartYear { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
} 