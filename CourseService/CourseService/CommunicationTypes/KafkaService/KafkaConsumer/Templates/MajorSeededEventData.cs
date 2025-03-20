namespace CourseService.CommunicationTypes.KafkaService.KafkaConsumer.Templates
{
    public class MajorSeededEventData
    {
        public List<MajorSingleData> Majors { get; set; }
    }

    public class MajorSingleData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    } 
}
