using System.ComponentModel.DataAnnotations;

namespace CourseService.CommunicationTypes.KafkaService.KafkaProducer.Templates
{
    public class ClassClosureEventDTO
    {
        public string EventType { get; set; } = "ClassClosureEvent";
        public string ServiceFrom { get; set; } = "CourseService";
        public ClassClosureEventData Data { get; set; }
    }

    public class ClassClosureEventData
    {
        public List<Guid> ClassIds { get; set; } = new List<Guid>();
    }
}
