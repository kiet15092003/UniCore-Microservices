using StudentService.Business.Dtos.Student;

namespace StudentService.Business.Services
{
    public interface IStudentSvc
    {
        Task<List<StudentReadDto>> GetAllStudents();

        Task<List<StudentReadDto>> AddStudentsFromExcel(IFormFile file);

        Task<StudentReadDto> CreateStudent(StudentCreateDto studentDto);

        Task<StudentReadDto> UpdateStudent(Guid id, StudentUpdateDto studentDto);

        Task<bool> DeleteStudent(Guid id);

        Task<List<StudentReadDto>> BulkCreateStudents(BulkCreateStudentRequest bulkCreateStudentDto);
    }
}
