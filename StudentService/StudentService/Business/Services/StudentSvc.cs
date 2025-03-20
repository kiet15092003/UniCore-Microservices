using AutoMapper;
using StudentService.Business.Dtos.Student;
using StudentService.DataAccess.Repositories.StudentRepo;

namespace StudentService.Business.Services
{
    public class StudentSvc : IStudentSvc
    {
        private readonly IStudentRepo _studentRepo;
        private readonly IMapper _mapper;
        public StudentSvc(IStudentRepo studentRepo, IMapper mapper)
        {
            _studentRepo = studentRepo;
            _mapper = mapper;
        }

        public async Task<List<StudentReadDto>> GetAllStudents()
        {
            var students = await _studentRepo.GetAllStudentsAsync();
            return _mapper.Map<List<StudentReadDto>>(students);
        }
    }
}
