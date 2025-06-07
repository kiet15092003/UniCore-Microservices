using AutoMapper;
using EnrollmentService.Business.Dtos.Enrollment;
using EnrollmentService.CommunicationTypes.Grpc.GrpcClient;
using EnrollmentService.DataAccess.Repositories;
using EnrollmentService.Entities;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;

namespace EnrollmentService.Business.Services
{
    public class EnrollmentSvc : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IMapper _mapper;
        private readonly GrpcAcademicClassClientService _academicClassClient;
        private readonly GrpcStudentClientService _studentClient;

        public EnrollmentSvc(
            IEnrollmentRepository enrollmentRepository,
            IMapper mapper,
            GrpcAcademicClassClientService academicClassClient,
            GrpcStudentClientService studentClient)
        {
            _enrollmentRepository = enrollmentRepository;
            _mapper = mapper;
            _academicClassClient = academicClassClient;
            _studentClient = studentClient;
        }

        public async Task<EnrollmentReadDto?> GetEnrollmentByIdAsync(Guid id)
        {
            var enrollment = await _enrollmentRepository.GetEnrollmentByIdAsync(id);
            if (enrollment == null)
            {
                return null;
            }

            var enrollmentDto = _mapper.Map<EnrollmentReadDto>(enrollment);            // Get student data
            var studentResponse = await _studentClient.GetStudentById(enrollment.StudentId.ToString());
            if (studentResponse.Success && studentResponse.Data != null)
            {
                var userData = studentResponse.Data.User;
                
                enrollmentDto.Student = new GrpcStudentData
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

            // Get academic class data
            var academicClassResponse = await _academicClassClient.GetAcademicClassById(enrollment.AcademicClassId.ToString());
            if (academicClassResponse.Success && academicClassResponse.Data != null)
            {
                var acClass = academicClassResponse.Data;
                
                enrollmentDto.AcademicClass = new GrpcAcademicClassData
                {
                    Id = Guid.Parse(acClass.Id),
                    Name = acClass.Name,
                    GroupNumber = acClass.GroupNumber,
                    Capacity = acClass.Capacity,
                    ListOfWeeks = acClass.ListOfWeeks?.ToList(),
                    IsRegistrable = acClass.IsRegistrable,
                    SemesterId = !string.IsNullOrEmpty(acClass.SemesterId) ? Guid.Parse(acClass.SemesterId) : Guid.Empty,
                    CourseId = !string.IsNullOrEmpty(acClass.CourseId) ? Guid.Parse(acClass.CourseId) : Guid.Empty,
                    
                    // Map Semester if available
                    Semester = acClass.Semester != null ? new GrpcSemesterData 
                    {
                        Id = Guid.Parse(acClass.Semester.Id),
                        SemesterNumber = acClass.Semester.SemesterNumber,
                        Year = acClass.Semester.Year,
                        IsActive = acClass.Semester.IsActive,
                        StartDate = !string.IsNullOrEmpty(acClass.Semester.StartDate) ? DateTime.Parse(acClass.Semester.StartDate) : DateTime.MinValue,
                        EndDate = !string.IsNullOrEmpty(acClass.Semester.EndDate) ? DateTime.Parse(acClass.Semester.EndDate) : DateTime.MinValue,
                        NumberOfWeeks = acClass.Semester.NumberOfWeeks
                    } : null,
                    
                    // Map Course if available
                    Course = acClass.Course != null ? new GrpcCourseData
                    {
                        Id = Guid.Parse(acClass.Course.Id),
                        Code = acClass.Course.Code,
                        Name = acClass.Course.Name,
                        Description = acClass.Course.Description,
                        IsActive = acClass.Course.IsActive,
                        Credit = acClass.Course.Credit,
                        PracticePeriod = acClass.Course.PracticePeriod,
                        IsRequired = acClass.Course.IsRequired,
                        Cost = acClass.Course.Cost
                    } : null,
                    
                    // Map ScheduleInDays if available
                    ScheduleInDays = acClass.ScheduleInDays?.Select(s => new GrpcScheduleInDayData
                    {
                        Id = Guid.Parse(s.Id),
                        DayOfWeek = s.DayOfWeek,
                        RoomId = !string.IsNullOrEmpty(s.RoomId) ? Guid.Parse(s.RoomId) : Guid.Empty,
                        ShiftId = !string.IsNullOrEmpty(s.ShiftId) ? Guid.Parse(s.ShiftId) : Guid.Empty,
                        
                        Room = s.Room != null ? new GrpcRoomData
                        {
                            Id = Guid.Parse(s.Room.Id),
                            Name = s.Room.Name,
                        } : null,
                        
                        Shift = s.Shift != null ? new GrpcShiftData
                        {
                            Id = Guid.Parse(s.Shift.Id),
                            Name = s.Shift.Name,
                            StartTime = s.Shift.StartTime,
                            EndTime = s.Shift.EndTime
                        } : null
                    }).ToList()
                };
            }

            return enrollmentDto;
        }

        public async Task<EnrollmentListResponse> GetEnrollmentsByPagination(Pagination pagination, EnrollmentListFilterParams filterParams, Order? order)
        {
            var enrollmentsResult = await _enrollmentRepository.GetAllEnrollmentsPaginationAsync(pagination, filterParams, order);
            
            var enrollmentDtos = _mapper.Map<List<EnrollmentReadDto>>(enrollmentsResult.Data);
              // Populate student and academic class data for each enrollment
            foreach (var enrollmentDto in enrollmentDtos)
            {
                // Get student data
                var studentResponse = await _studentClient.GetStudentById(enrollmentDto.StudentId.ToString());
                if (studentResponse.Success && studentResponse.Data != null)
                {
                    var userData = studentResponse.Data.User;
                    
                    enrollmentDto.Student = new GrpcStudentData
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
                }                // Get academic class data
                var academicClassResponse = await _academicClassClient.GetAcademicClassById(enrollmentDto.AcademicClassId.ToString());
                if (academicClassResponse.Success && academicClassResponse.Data != null)
                {
                    var acClass = academicClassResponse.Data;
                    
                    enrollmentDto.AcademicClass = new GrpcAcademicClassData
                    {
                        Id = Guid.Parse(acClass.Id),
                        Name = acClass.Name,
                        GroupNumber = acClass.GroupNumber,
                        Capacity = acClass.Capacity,
                        ListOfWeeks = acClass.ListOfWeeks?.ToList(),
                        IsRegistrable = acClass.IsRegistrable,
                        CourseId = !string.IsNullOrEmpty(acClass.CourseId) ? Guid.Parse(acClass.CourseId) : Guid.Empty,
                        SemesterId = !string.IsNullOrEmpty(acClass.SemesterId) ? Guid.Parse(acClass.SemesterId) : Guid.Empty,
                        
                        // Map Semester if available
                        Semester = acClass.Semester != null ? new GrpcSemesterData 
                        {
                            Id = Guid.Parse(acClass.Semester.Id),
                            SemesterNumber = acClass.Semester.SemesterNumber,
                            Year = acClass.Semester.Year,
                            IsActive = acClass.Semester.IsActive,
                            StartDate = !string.IsNullOrEmpty(acClass.Semester.StartDate) ? DateTime.Parse(acClass.Semester.StartDate) : DateTime.MinValue,
                            EndDate = !string.IsNullOrEmpty(acClass.Semester.EndDate) ? DateTime.Parse(acClass.Semester.EndDate) : DateTime.MinValue,
                            NumberOfWeeks = acClass.Semester.NumberOfWeeks
                        } : null,
                        
                        // Map Course if available with complete data
                        Course = acClass.Course != null ? new GrpcCourseData
                        {
                            Id = Guid.Parse(acClass.Course.Id),
                            Code = acClass.Course.Code,
                            Name = acClass.Course.Name,
                            Description = acClass.Course.Description,
                            IsActive = acClass.Course.IsActive,
                            Credit = acClass.Course.Credit,
                            PracticePeriod = acClass.Course.PracticePeriod,
                            IsRequired = acClass.Course.IsRequired,
                            Cost = acClass.Course.Cost
                        } : null,
                        
                        // Map ScheduleInDays if available
                        ScheduleInDays = acClass.ScheduleInDays?.Select(s => new GrpcScheduleInDayData
                        {
                            Id = Guid.Parse(s.Id),
                            DayOfWeek = s.DayOfWeek,
                            RoomId = !string.IsNullOrEmpty(s.RoomId) ? Guid.Parse(s.RoomId) : Guid.Empty,
                            ShiftId = !string.IsNullOrEmpty(s.ShiftId) ? Guid.Parse(s.ShiftId) : Guid.Empty,
                            
                            Room = s.Room != null ? new GrpcRoomData
                            {
                                Id = Guid.Parse(s.Room.Id),
                                Name = s.Room.Name,
                            } : null,
                            
                            Shift = s.Shift != null ? new GrpcShiftData
                            {
                                Id = Guid.Parse(s.Shift.Id),
                                Name = s.Shift.Name,
                                StartTime = s.Shift.StartTime,
                                EndTime = s.Shift.EndTime
                            } : null
                        }).ToList()
                    };
                }
            }            return new EnrollmentListResponse
            {
                Data = enrollmentDtos,
                Total = enrollmentsResult.Total,
                PageSize = enrollmentsResult.PageSize,
                PageIndex = enrollmentsResult.PageIndex,
            };
        }        public async Task<List<EnrollmentReadDto>> CreateMultipleEnrollmentsAsync(MultipleEnrollmentCreateDto createDto)
        {
            var successfulEnrollments = new List<EnrollmentReadDto>();
            var enrollmentsToCreate = new List<Enrollment>();
            
            // Validate student exists
            var studentResponse = await _studentClient.GetStudentById(createDto.StudentId.ToString());
            if (!studentResponse.Success || studentResponse.Data == null)
            {
                // If student doesn't exist, return empty list
                return successfulEnrollments;
            }

            // Process each academic class
            foreach (var academicClassId in createDto.AcademicClassIds)
            {
                try
                {
                    // Check if enrollment already exists
                    var existingEnrollment = await _enrollmentRepository.ExistsAsync(createDto.StudentId, academicClassId);
                    if (existingEnrollment)
                    {
                        continue; // Skip existing enrollment
                    }

                    // Validate academic class exists and is registrable
                    var academicClassResponse = await _academicClassClient.GetAcademicClassById(academicClassId.ToString());
                    if (!academicClassResponse.Success || academicClassResponse.Data == null || !academicClassResponse.Data.IsRegistrable)
                    {
                        continue; // Skip invalid or non-registrable class
                    }

                    // Create enrollment entity
                    var enrollment = new Enrollment
                    {
                        StudentId = createDto.StudentId,
                        AcademicClassId = academicClassId,
                        Status = 1, // Set status to 1 as requested
                        StudentResults = new List<StudentResult>()
                    };

                    enrollmentsToCreate.Add(enrollment);
                }
                catch
                {
                    // Skip on error
                    continue;
                }
            }

            // Create all valid enrollments in batch
            if (enrollmentsToCreate.Any())
            {
                try
                {
                    var createdEnrollments = await _enrollmentRepository.CreateMultipleEnrollmentsAsync(enrollmentsToCreate);
                    
                    // Convert to DTOs and populate additional data
                    foreach (var createdEnrollment in createdEnrollments)
                    {
                        var enrollmentDto = _mapper.Map<EnrollmentReadDto>(createdEnrollment);
                        
                        // Populate student data
                        if (studentResponse.Success && studentResponse.Data != null)
                        {
                            var userData = studentResponse.Data.User;
                            enrollmentDto.Student = new GrpcStudentData
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

                        // Populate academic class data
                        var academicClassResponse = await _academicClassClient.GetAcademicClassById(createdEnrollment.AcademicClassId.ToString());
                        if (academicClassResponse.Success && academicClassResponse.Data != null)
                        {
                            var acClass = academicClassResponse.Data;
                            enrollmentDto.AcademicClass = new GrpcAcademicClassData
                            {
                                Id = Guid.Parse(acClass.Id),
                                Name = acClass.Name,
                                GroupNumber = acClass.GroupNumber,
                                Capacity = acClass.Capacity,
                                ListOfWeeks = acClass.ListOfWeeks?.ToList(),
                                IsRegistrable = acClass.IsRegistrable,
                                SemesterId = !string.IsNullOrEmpty(acClass.SemesterId) ? Guid.Parse(acClass.SemesterId) : Guid.Empty,
                                CourseId = !string.IsNullOrEmpty(acClass.CourseId) ? Guid.Parse(acClass.CourseId) : Guid.Empty,
                                
                                Course = acClass.Course != null ? new GrpcCourseData
                                {
                                    Id = Guid.Parse(acClass.Course.Id),
                                    Code = acClass.Course.Code,
                                    Name = acClass.Course.Name,
                                    Description = acClass.Course.Description,
                                    IsActive = acClass.Course.IsActive,
                                    Credit = acClass.Course.Credit,
                                    PracticePeriod = acClass.Course.PracticePeriod,
                                    IsRequired = acClass.Course.IsRequired,
                                    Cost = acClass.Course.Cost
                                } : null
                            };
                        }

                        successfulEnrollments.Add(enrollmentDto);
                    }
                }
                catch
                {
                    // Return empty list on database error
                    return new List<EnrollmentReadDto>();
                }
            }            return successfulEnrollments;
        }

        public async Task<int> GetEnrollmentCountByAcademicClassIdAsync(Guid academicClassId)
        {
            return await _enrollmentRepository.GetEnrollmentCountByAcademicClassIdAsync(academicClassId);
        }

        public async Task<bool> CheckEnrollmentExistsAsync(Guid studentId, Guid academicClassId)
        {
            return await _enrollmentRepository.ExistsAsync(studentId, academicClassId);
        }

        public async Task<CheckMultipleEnrollmentsResponse> CheckMultipleEnrollmentsExistAsync(CheckMultipleEnrollmentsRequest request)
        {
            var results = new List<EnrollmentExistenceResult>();

            foreach (var academicClassId in request.AcademicClassIds)
            {
                var exists = await _enrollmentRepository.ExistsAsync(request.StudentId, academicClassId);
                results.Add(new EnrollmentExistenceResult
                {
                    AcademicClassId = academicClassId,
                    Exists = exists
                });
            }

            return new CheckMultipleEnrollmentsResponse
            {
                Results = results
            };
        }

        public async Task<List<EnrollmentReadDto>> GetEnrollmentsByStudentIdAsync(Guid studentId)
        {
            var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentIdAsync(studentId);
            var enrollmentDtos = _mapper.Map<List<EnrollmentReadDto>>(enrollments);

            // Populate student and academic class data for each enrollment
            foreach (var enrollmentDto in enrollmentDtos)
            {
                // Get student data
                var studentResponse = await _studentClient.GetStudentById(enrollmentDto.StudentId.ToString());
                if (studentResponse.Success && studentResponse.Data != null)
                {
                    var userData = studentResponse.Data.User;
                    
                    enrollmentDto.Student = new GrpcStudentData
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

                // Get academic class data
                var academicClassResponse = await _academicClassClient.GetAcademicClassById(enrollmentDto.AcademicClassId.ToString());
                if (academicClassResponse.Success && academicClassResponse.Data != null)
                {
                    var acClass = academicClassResponse.Data;
                    
                    enrollmentDto.AcademicClass = new GrpcAcademicClassData
                    {
                        Id = Guid.Parse(acClass.Id),
                        Name = acClass.Name,
                        GroupNumber = acClass.GroupNumber,
                        Capacity = acClass.Capacity,
                        ListOfWeeks = acClass.ListOfWeeks?.ToList(),
                        IsRegistrable = acClass.IsRegistrable,
                        CourseId = !string.IsNullOrEmpty(acClass.CourseId) ? Guid.Parse(acClass.CourseId) : Guid.Empty,
                        SemesterId = !string.IsNullOrEmpty(acClass.SemesterId) ? Guid.Parse(acClass.SemesterId) : Guid.Empty,
                        
                        Course = acClass.Course != null ? new GrpcCourseData
                        {
                            Id = Guid.Parse(acClass.Course.Id),
                            Code = acClass.Course.Code,
                            Name = acClass.Course.Name,
                            Description = acClass.Course.Description,
                            IsActive = acClass.Course.IsActive,
                            Credit = acClass.Course.Credit,
                            PracticePeriod = acClass.Course.PracticePeriod,
                            IsRequired = acClass.Course.IsRequired,
                            Cost = acClass.Course.Cost
                        } : null,
                        
                        ScheduleInDays = acClass.ScheduleInDays?.Select(s => new GrpcScheduleInDayData
                        {
                            Id = Guid.Parse(s.Id),
                            DayOfWeek = s.DayOfWeek,
                            RoomId = !string.IsNullOrEmpty(s.RoomId) ? Guid.Parse(s.RoomId) : Guid.Empty,
                            ShiftId = !string.IsNullOrEmpty(s.ShiftId) ? Guid.Parse(s.ShiftId) : Guid.Empty,
                            
                            Room = s.Room != null ? new GrpcRoomData
                            {
                                Id = Guid.Parse(s.Room.Id),
                                Name = s.Room.Name,
                            } : null,
                            
                            Shift = s.Shift != null ? new GrpcShiftData
                            {
                                Id = Guid.Parse(s.Shift.Id),
                                Name = s.Shift.Name,
                                StartTime = s.Shift.StartTime,
                                EndTime = s.Shift.EndTime
                            } : null
                        }).ToList()
                    };
                }
            }

            return enrollmentDtos;
        }
    }
}
