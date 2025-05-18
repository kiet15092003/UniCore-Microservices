using System.ComponentModel.DataAnnotations;

namespace UserService.CommunicationTypes.KafkaService.KafkaProducer.Templates
{
    public class UserImportedEventDTO
    {
        public string EventType { get; set; } = "UserImportedEvent";
        public string ServiceFrom { get; set; } = "UserService";
        public UserImportedEventData Data { get; set; }
    }

    public class UserImportedEventData
    {
        public List<UserImportedEventDataSingleData> Users { get; set; }
    }
    
    public class UserImportedEventDataSingleData
    {
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string Password { get; set; }    
        [Required]
        public string PhoneNumber { get; set; } 
    }
}
