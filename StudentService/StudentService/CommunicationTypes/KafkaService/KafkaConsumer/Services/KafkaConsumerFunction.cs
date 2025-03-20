using StudentService.CommunicationTypes.KafkaService.KafkaConsumer.Templates;
using StudentService.DataAccess.Repositories.StudentRepo;
using StudentService.Entities;

namespace StudentService.CommunicationTypes.KafkaService.KafkaConsumer.Services
{
    public class KafkaConsumerFunction : IKafkaConsumerFunction
    {
        private readonly IStudentRepo _studentRepo;

        public KafkaConsumerFunction(
            IStudentRepo studentRepo)
        {
            _studentRepo = studentRepo;
        }

        public async Task<Student> CreateStudent(StudentCreatedEventData data)
        {
            return await _studentRepo.CreateStudentAsync(new Student
            {
                StudentCode = data.StudentCode,
                ApplicationUserId = data.ApplicationUserId,
                MajorId = data.MajorId,
                BatchId = data.BatchId,
            });
        }
    }
}
