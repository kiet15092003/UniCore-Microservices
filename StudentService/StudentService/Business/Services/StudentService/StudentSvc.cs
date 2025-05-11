using AutoMapper;
using ClosedXML.Excel;
using StudentService.Business.Dtos.Student;
using StudentService.DataAccess.Repositories.StudentRepo;
using StudentService.Entities;

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

        public async Task<List<StudentReadDto>> AddStudentsFromExcel(IFormFile file)
        {
            var students = new List<Student>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Get first worksheet
                    var rowCount = worksheet.LastRowUsed().RowNumber();

                    for (int row = 2; row <= rowCount; row++) // Assuming first row is header
                    {
                        var student = new Student
                        {
                            StudentCode = worksheet.Cell(row, 1).GetString(),
                            AccumulateCredits = worksheet.Cell(row, 2).GetValue<int>(),
                            AccumulateScore = worksheet.Cell(row, 3).GetValue<double>(),
                            AccumulateActivityScore = worksheet.Cell(row, 4).GetValue<int>(),
                            MajorId = Guid.Parse(worksheet.Cell(row, 5).GetString()),
                            BatchId = Guid.Parse(worksheet.Cell(row, 6).GetString()),
                            ApplicationUserId = Guid.Parse(worksheet.Cell(row, 7).GetString())
                        };
                        students.Add(student);
                    }
                }
            }

            await _studentRepo.AddRangeAsync(students);
            return _mapper.Map<List<StudentReadDto>>(students);
        }

        public async Task<StudentReadDto> CreateStudent(StudentCreateDto studentDto)
        {
            var student = _mapper.Map<Student>(studentDto);
            var createdStudent = await _studentRepo.CreateStudentAsync(student);
            return _mapper.Map<StudentReadDto>(createdStudent);
        }

        public async Task<StudentReadDto> UpdateStudent(Guid id, StudentUpdateDto studentDto)
        {
            var existingStudent = await _studentRepo.GetStudentByIdAsync(id);
            if (existingStudent == null)
            {
                throw new KeyNotFoundException($"Student with ID {id} not found.");
            }

            _mapper.Map(studentDto, existingStudent);
            await _studentRepo.UpdateStudentAsync(existingStudent);
            return _mapper.Map<StudentReadDto>(existingStudent);
        }

        public async Task<bool> DeleteStudent(Guid id)
        {
            var student = await _studentRepo.GetStudentByIdAsync(id);
            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {id} not found.");
            }

            await _studentRepo.DeleteStudentAsync(student);
            return true;
        }
        
        public async Task<List<StudentReadDto>> BulkCreateStudents(BulkCreateStudentRequest bulkCreateStudentRequest)
        {
            var students = _mapper.Map<List<Student>>(bulkCreateStudentRequest.Students);
            await _studentRepo.AddRangeAsync(students);
            await _studentRepo.SaveChangesAsync();
            return _mapper.Map<List<StudentReadDto>>(students);
        }
    }
}
