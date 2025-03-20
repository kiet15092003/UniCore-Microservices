namespace StudentService.CommunicationTypes.KafkaService.KafkaConsumer.Templates
{
    public class BaseEvent
    {
        public string EventType { get; set; } = string.Empty;
        public string ServiceFrom { get; set; } = string.Empty;
        public object Data { get; set; } = new(); 
    }

}
