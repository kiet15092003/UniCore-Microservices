using System.ComponentModel.DataAnnotations;

namespace UserService.CommunicationTypes.KafkaService.KafkaProducer.Templates
{
    public class StudentCreatedEventDTO
    {
        public string EventType { get; set; } = "StudentCreatedEvent";
        public string ServiceFrom { get; set; } = "UserService";
        public StudentCreatedEventData Data { get; set; }
    }

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
