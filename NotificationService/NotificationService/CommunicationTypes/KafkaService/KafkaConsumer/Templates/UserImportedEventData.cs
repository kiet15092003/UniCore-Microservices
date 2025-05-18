using System.ComponentModel.DataAnnotations;

namespace StudentService.CommunicationTypes.KafkaService.KafkaConsumer.Templates
{
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
