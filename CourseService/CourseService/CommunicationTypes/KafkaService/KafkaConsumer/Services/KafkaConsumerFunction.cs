using CourseService.CommunicationTypes.KafkaService.KafkaConsumer.Templates;
using CourseService.DataAccess.Repositories.MajorRepo;
using CourseService.DataAccess.Repositories.StudentRepo;
using CourseService.DataAccess.Repositories.TrainingManagerRepo;
using CourseService.Entities;

namespace CourseService.CommunicationTypes.KafkaService.KafkaConsumer.Services
{
    public class KafkaConsumerFunction : IKafkaConsumerFunction
    {
        private readonly ITrainingManagerRepo _trainingManagerRepo;
        private readonly IMajorRepo _majorRepo;
        private readonly IStudentRepo _studentRepo;

        public KafkaConsumerFunction(
            ITrainingManagerRepo trainingManagerRepo, 
            IMajorRepo repo,
            IStudentRepo studentRepo)
        {
            _trainingManagerRepo = trainingManagerRepo;
            _majorRepo = repo;
            _studentRepo = studentRepo;
        }

        public async Task<Student> CreateStudent(StudentCreatedEventData data)
        {
            return await _studentRepo.CreateStudentAsync(new Student
            {
                Id = data.Id,
                Email = data.Email,
                FullName = data.FullName,
                StudentCode = data.StudentCode,
            });
        }

        public async Task<TrainingManager> CreateTrainingManager(TrainingManagerCreatedEventData data)
        {
            return await _trainingManagerRepo.CreateTrainingManagerAsync(new TrainingManager
            {
                Id = data.Id,
                Email = data.Email,
                FullName = data.FullName,
                TrainingManagerCode = data.TrainingManagerCode,
            });
        }

        public async Task SeedMajors(MajorSeededEventData data)
        {
            foreach (var major in data.Majors)
            {
                await _majorRepo.CreateMajorAsync(new Major
                {
                    Id = major.Id,
                    Name = major.Name,
                });
            }
        }
    }
}
