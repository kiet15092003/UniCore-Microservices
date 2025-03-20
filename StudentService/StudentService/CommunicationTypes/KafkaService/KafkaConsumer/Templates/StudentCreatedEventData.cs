using System.ComponentModel.DataAnnotations;

namespace StudentService.CommunicationTypes.KafkaService.KafkaConsumer.Templates
{
    public class StudentCreatedEventData
    {
        [Required]
        public string StudentCode { get; set; }
        public int TotalCredits { get; set; }

        [Required]
        public Guid ApplicationUserId { get; set; }
        [Required]
        public Guid MajorId { get; set; }
        [Required]
        public Guid BatchId { get; set; }
    }
}
