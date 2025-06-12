namespace EnrollmentService.CommunicationTypes.KafkaService.KafkaConsumer.Templates
{
    public class ClassClosureEventData
    {
        public string EventType { get; set; } = string.Empty;
        public string ServiceFrom { get; set; } = string.Empty;
        public ClassClosureEventDataContent Data { get; set; } = new ClassClosureEventDataContent();
    }

    public class ClassClosureEventDataContent
    {
        public List<Guid> ClassIds { get; set; } = new List<Guid>();
    }
}
