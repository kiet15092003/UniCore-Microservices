using AutoMapper;
using EnrollmentService.Business.Dtos.Enrollment;
using EnrollmentService.Business.Dtos.Exam;
using EnrollmentService.CommunicationTypes.Grpc.GrpcClient;
using EnrollmentService.DataAccess.Repositories;
using EnrollmentService.Entities;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;

namespace EnrollmentService.Business.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IMapper _mapper;
        private readonly GrpcRoomClientService _roomClient;
        private readonly GrpcAcademicClassClientService _academicClassClient;
        private readonly ILogger<ExamService> _logger;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly GrpcStudentClientService _studentClient;
        public ExamService(
            IExamRepository examRepository,
            IMapper mapper,
            GrpcRoomClientService roomClient,
            GrpcAcademicClassClientService academicClassClient,
            ILogger<ExamService> logger,
            IEnrollmentRepository enrollmentRepository,
            GrpcStudentClientService studentClient)
        {
            _examRepository = examRepository;
            _mapper = mapper;
            _roomClient = roomClient;
            _academicClassClient = academicClassClient;
            _logger = logger;
            _enrollmentRepository = enrollmentRepository;
            _studentClient = studentClient;
        }
        public async Task<ExamReadDto?> GetExamByIdAsync(Guid id)
        {
            var exam = await _examRepository.GetExamByIdAsync(id);
            if (exam == null)
                return null;

            var examDto = _mapper.Map<ExamReadDto>(exam);

            examDto.EnrollmentExams = await BuildEnrollmentExamReadDtos(exam.EnrollmentExams);

            await PopulateExamDataAsync(examDto);
            PopulateExamStatisticsFromEntity(examDto, exam);
            return examDto;
        }

        private async Task<List<EnrollmentExamReadDto>> BuildEnrollmentExamReadDtos(List<EnrollmentExam> enrollmentExams)
        {
            var result = new List<EnrollmentExamReadDto>();
            foreach (var enrollmentExam in enrollmentExams)
            {
                var enrollment = await _enrollmentRepository.GetEnrollmentByIdAsync(enrollmentExam.EnrollmentId);
                var student = await PopulateStudentDataForEnrollmentExamsAsync(enrollment!.StudentId);
                result.Add(new EnrollmentExamReadDto
                {
                    StudentId = enrollment!.StudentId,
                    EnrollmentId = enrollment!.Id,
                    Student = student
                });
            }
            return result;
        }

        private async Task<GrpcStudentData> PopulateStudentDataForEnrollmentExamsAsync(Guid StudentId)
        {

            var studentResponse = await _studentClient.GetStudentById(StudentId.ToString());
            if (studentResponse.Success && studentResponse.Data != null)
            {
                var userData = studentResponse.Data.User;
                return new GrpcStudentData
                {
                    Id = Guid.Parse(studentResponse.Data.Id),
                    StudentCode = studentResponse.Data.StudentCode,
                    AccumulateCredits = studentResponse.Data.AccumulateCredits,
                    AccumulateScore = studentResponse.Data.AccumulateScore,
                    AccumulateActivityScore = studentResponse.Data.AccumulateActivityScore,
                    MajorId = !string.IsNullOrEmpty(studentResponse.Data.MajorId) ? Guid.Parse(studentResponse.Data.MajorId) : Guid.Empty,
                    BatchId = !string.IsNullOrEmpty(studentResponse.Data.BatchId) ? Guid.Parse(studentResponse.Data.BatchId) : Guid.Empty,
                    ApplicationUserId = !string.IsNullOrEmpty(studentResponse.Data.ApplicationUserId) ? Guid.Parse(studentResponse.Data.ApplicationUserId) : Guid.Empty,
                    User = userData != null ? new GrpcUserData
                    {
                        Id = !string.IsNullOrEmpty(userData.Id) ? Guid.Parse(userData.Id) : Guid.Empty,
                        FirstName = userData.FirstName,
                        LastName = userData.LastName,
                        Email = userData.Email,
                        PhoneNumber = userData.PhoneNumber,
                        PersonId = userData.PersonId,
                        ImageUrl = userData.ImageUrl
                    } : null
                };
                
            }
            return null;              
        }

        public async Task<List<ExamReadDto>> GetExamsByAcademicClassIdAsync(Guid academicClassId)
        {
            var exams = await _examRepository.GetExamsByAcademicClassIdAsync(academicClassId);
            var examDtos = new List<ExamReadDto>();
            
            foreach (var exam in exams)
            {
                var examDto = _mapper.Map<ExamReadDto>(exam);
                await PopulateExamDataAsync(examDto);
                PopulateExamStatisticsFromEntity(examDto, exam);
                examDtos.Add(examDto);
            }
            
            return examDtos;
        }

        public async Task<ExamReadDto> CreateExamAsync(ExamCreateDto createDto)
        {
            // Validate room and academic class exist via gRPC calls
            await ValidateExamDataAsync(createDto.RoomId, createDto.AcademicClassId);

            var exam = _mapper.Map<Exam>(createDto);
            var createdExam = await _examRepository.CreateExamAsync(exam);
            
            var examDto = _mapper.Map<ExamReadDto>(createdExam);
            await PopulateExamDataAsync(examDto);
            await PopulateExamStatisticsAsync(examDto);
            return examDto;
        }

        public async Task<ExamReadDto> UpdateExamAsync(Guid id, ExamCreateDto updateDto)
        {
            var exam = await _examRepository.GetExamByIdAsync(id);
            if (exam == null)
                throw new ArgumentException($"Exam with ID {id} not found");

            // Validate room and academic class exist via gRPC calls
            await ValidateExamDataAsync(updateDto.RoomId, updateDto.AcademicClassId);

            _mapper.Map(updateDto, exam);
            var updatedExam = await _examRepository.UpdateExamAsync(exam);
            
            var examDto = _mapper.Map<ExamReadDto>(updatedExam);
            await PopulateExamDataAsync(examDto);
            await PopulateExamStatisticsAsync(examDto);
            return examDto;
        }

        public async Task<bool> DeleteExamAsync(Guid id)
        {
            return await _examRepository.DeleteExamAsync(id);
        }

        public async Task<List<ExamReadDto>> GetAllExamsAsync()
        {
            var exams = await _examRepository.GetAllExamsAsync();
            var examDtos = _mapper.Map<List<ExamReadDto>>(exams);
            
            foreach (var examDto in examDtos)
            {
                await PopulateExamDataAsync(examDto);
            }
            
            return examDtos;
        }        
        public async Task<ExamListResponse> GetExamsByPagination(Pagination pagination, ExamListFilterParams filterParams, Order? order)
        {
            var examsResult = await _examRepository.GetAllExamsPaginationAsync(pagination, filterParams, order);
            
            var examDtos = new List<ExamReadDto>();
            
            // Process each exam and populate data
            foreach (var exam in examsResult.Data)
            {
                var examDto = _mapper.Map<ExamReadDto>(exam);
                await PopulateExamDataAsync(examDto);
                PopulateExamStatisticsFromEntity(examDto, exam);
                examDtos.Add(examDto);
            }
            
            return new ExamListResponse
            {
                Data = examDtos,
                Total = examsResult.Total,
                PageSize = examsResult.PageSize,
                PageIndex = examsResult.PageIndex
            };
        }

        private async Task PopulateExamDataAsync(ExamReadDto examDto)
        {
            try
            {
                // Get room data
                var roomResponse = await _roomClient.GetRoomByIdAsync(examDto.RoomId.ToString());
                if (roomResponse.Success && roomResponse.Data != null)
                {
                    examDto.Room = new ExamGrpcRoomData
                    {
                        Id = Guid.Parse(roomResponse.Data.Id),
                        Name = roomResponse.Data.Name,
                        AvailableSeats = roomResponse.Data.AvailableSeats,
                        FloorId = !string.IsNullOrEmpty(roomResponse.Data.FloorId) ? Guid.Parse(roomResponse.Data.FloorId) : Guid.Empty
                    };
                }                // Get academic class data
                var classResponse = await _academicClassClient.GetAcademicClassById(examDto.AcademicClassId.ToString());
                if (classResponse.Success && classResponse.Data != null)
                {
                    examDto.AcademicClass = new ExamGrpcAcademicClassData
                    {
                        Id = Guid.Parse(classResponse.Data.Id),
                        Name = classResponse.Data.Name,
                        GroupNumber = classResponse.Data.GroupNumber,
                        Capacity = classResponse.Data.Capacity,
                        ListOfWeeks = classResponse.Data.ListOfWeeks?.ToList(),
                        IsRegistrable = classResponse.Data.IsRegistrable,
                        SemesterId = !string.IsNullOrEmpty(classResponse.Data.SemesterId) ? Guid.Parse(classResponse.Data.SemesterId) : Guid.Empty,
                        CourseId = !string.IsNullOrEmpty(classResponse.Data.CourseId) ? Guid.Parse(classResponse.Data.CourseId) : Guid.Empty
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to populate exam data for exam {ExamId}", examDto.Id);
            }
        }

        private async Task ValidateExamDataAsync(Guid roomId, Guid academicClassId)
        {
            // Validate room exists
            var roomResponse = await _roomClient.GetRoomByIdAsync(roomId.ToString());
            if (!roomResponse.Success || roomResponse.Data == null)
            {
                throw new ArgumentException($"Room with ID {roomId} not found");
            }

            // Validate academic class exists
            var classResponse = await _academicClassClient.GetAcademicClassById(academicClassId.ToString());
            if (!classResponse.Success || classResponse.Data == null)
            {
                throw new ArgumentException($"Academic class with ID {academicClassId} not found");
            }
        }        
        private async Task PopulateExamStatisticsAsync(ExamReadDto examDto)
        {
            try
            {
                var examWithEnrollments = await _examRepository.GetExamByIdAsync(examDto.Id);
                if (examWithEnrollments?.EnrollmentExams != null)
                {
                    var enrollmentExams = examWithEnrollments.EnrollmentExams;
                    
                    examDto.TotalEnrollment = enrollmentExams.Count;                   
                }
                else
                {
                    examDto.TotalEnrollment = 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to populate exam statistics for exam {ExamId}", examDto.Id);
                examDto.TotalEnrollment = 0;
            }
        }

        private void PopulateExamStatisticsFromEntity(ExamReadDto examDto, Exam exam)
        {
            if (exam.EnrollmentExams != null)
            {
                var enrollmentExams = exam.EnrollmentExams;
                
                examDto.TotalEnrollment = enrollmentExams.Count;
            }
            else
            {
                examDto.TotalEnrollment = 0;
            }
        }

        public async Task<List<EnrollmentExamDto>> AddEnrollmentToExamAsync(AddEnrollmentToExamDto addEnrollmentDto)
        {
            try
            {
                // Validate that the exam exists
                var exam = await _examRepository.GetExamByIdAsync(addEnrollmentDto.ExamId);
                if (exam == null)
                {
                    throw new ArgumentException($"Exam with ID {addEnrollmentDto.ExamId} not found");
                }

                // Check for duplicate enrollments
                var existingEnrollmentIds = exam.EnrollmentExams?.Select(ee => ee.EnrollmentId).ToList() ?? new List<Guid>();
                var duplicateIds = addEnrollmentDto.EnrollmentIds.Where(id => existingEnrollmentIds.Contains(id)).ToList();
                
                if (duplicateIds.Any())
                {
                    _logger.LogWarning("Duplicate enrollment IDs found for exam {ExamId}: {DuplicateIds}", 
                        addEnrollmentDto.ExamId, string.Join(", ", duplicateIds));
                }

                // Filter out duplicates
                var newEnrollmentIds = addEnrollmentDto.EnrollmentIds.Except(existingEnrollmentIds).ToList();

                if (!newEnrollmentIds.Any())
                {
                    _logger.LogInformation("No new enrollments to add for exam {ExamId}", addEnrollmentDto.ExamId);
                    return new List<EnrollmentExamDto>();
                }

                // Create EnrollmentExam entities
                var enrollmentExams = new List<EnrollmentExam>();
                foreach (var enrollmentId in newEnrollmentIds)
                {
                    var enrollmentExam = new EnrollmentExam
                    {
                        ExamId = addEnrollmentDto.ExamId,
                        EnrollmentId = enrollmentId,
                    };
                    enrollmentExams.Add(enrollmentExam);
                }

                // Save to database
                var createdEnrollmentExams = await _examRepository.CreateEnrollmentExamsAsync(enrollmentExams);

                // Map to DTOs and return
                var result = _mapper.Map<List<EnrollmentExamDto>>(createdEnrollmentExams);
                _logger.LogInformation("Successfully added {Count} enrollments to exam {ExamId}", result.Count, addEnrollmentDto.ExamId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding enrollments to exam {ExamId}", addEnrollmentDto.ExamId);
                throw;
            }
        }

        public async Task<List<ExamReadDto>> GetExamsByStudentIdAsync(Guid studentId)
        {
            // Get all enrollments for the student
            var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentIdAsync(studentId);
            if (enrollments == null || enrollments.Count == 0)
                return new List<ExamReadDto>();

            // Get all enrollment IDs
            var enrollmentIds = enrollments.Select(e => e.Id).ToList();

            // Use repository method to get exams for these enrollments
            var examsForStudent = await _examRepository.GetExamsByEnrollmentIdsAsync(enrollmentIds);

            var examDtos = _mapper.Map<List<ExamReadDto>>(examsForStudent);
            foreach (var examDto in examDtos)
            {
                await PopulateExamDataAsync(examDto);
            }
            return examDtos;
        }
    }
}
