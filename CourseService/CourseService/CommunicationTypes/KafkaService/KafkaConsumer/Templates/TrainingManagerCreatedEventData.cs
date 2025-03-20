namespace CourseService.CommunicationTypes.KafkaService.KafkaConsumer.Templates
{
    public class TrainingManagerCreatedEventData
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string TrainingManagerCode { get; set; }
        public string FullName { get; set; }
    }
}
