namespace CourseService.CommunicationTypes.KafkaService.KafkaConsumer.Templates
{
    public class StudentCreatedEventData
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string StudentCode { get; set; }
        public string FullName { get; set; }
    }
}
