using AutoMapper;
using CourseService.Business.Dtos.Student;
using CourseService.DataAccess.Repositories.StudentRepo;
using CourseService.Entities;

namespace CourseService.Business.Services.StudentService
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepo _studentRepo;
        private readonly IMapper _mapper;
        public StudentService(IStudentRepo studentRepo, IMapper mapper)
        {
            _studentRepo = studentRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StudentReadDto>> GetAllStudentAsync()
        {
            var students = await _studentRepo.GetAllStudentAsync();
            return _mapper.Map<IEnumerable<StudentReadDto>>(students);
        }
    }
}
