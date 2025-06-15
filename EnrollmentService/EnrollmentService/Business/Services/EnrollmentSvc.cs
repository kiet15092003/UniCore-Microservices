using AutoMapper;
using EnrollmentService.Business.Dtos.Enrollment;
using EnrollmentService.CommunicationTypes.Grpc.GrpcClient;
using EnrollmentService.DataAccess.Repositories;
using EnrollmentService.Entities;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;
using EnrollmentService.Utils.DistributedLock;
using EnrollmentService.Utils.Exceptions;
using EnrollmentService.Utils.DistributedLock;
using EnrollmentService.Utils.Exceptions;

namespace EnrollmentService.Business.Services
{    public class EnrollmentSvc : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IMapper _mapper;
        private readonly GrpcAcademicClassClientService _academicClassClient;
        private readonly GrpcStudentClientService _studentClient;
        private readonly IDistributedLockService _distributedLockService;
        private readonly ILogger<EnrollmentSvc> _logger;

        public EnrollmentSvc(
            IEnrollmentRepository enrollmentRepository,
            IMapper mapper,
            GrpcAcademicClassClientService academicClassClient,
            GrpcStudentClientService studentClient,
            IDistributedLockService distributedLockService,
            ILogger<EnrollmentSvc> logger)
        {
            _enrollmentRepository = enrollmentRepository;
            _mapper = mapper;
            _academicClassClient = academicClassClient;
            _studentClient = studentClient;
            _distributedLockService = distributedLockService;
            _logger = logger;
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
                    Id = Guid.Parse(acClass.Id),                    Name = acClass.Name,
                    GroupNumber = acClass.GroupNumber,
                    Capacity = acClass.Capacity,
                    ListOfWeeks = acClass.ListOfWeeks != null ? new List<int>(acClass.ListOfWeeks) : null,
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
                        ListOfWeeks = acClass.ListOfWeeks != null ? new List<int>(acClass.ListOfWeeks) : null,
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
        }
        public async Task<List<EnrollmentReadDto>> CreateMultipleEnrollmentsAsync(MultipleEnrollmentCreateDto createDto)
        {
            _logger.LogInformation("Starting all-or-nothing multiple enrollment creation for student {StudentId} with {Count} academic classes",
                createDto.StudentId, createDto.AcademicClassIds.Count);

            // Validate student exists first
            var studentResponse = await _studentClient.GetStudentById(createDto.StudentId.ToString());
            if (!studentResponse.Success || studentResponse.Data == null)
            {
                _logger.LogWarning("Student {StudentId} not found", createDto.StudentId);
                throw new ArgumentException($"Student with ID {createDto.StudentId} not found");
            }

            // Acquire distributed locks for ALL academic classes first
            var distributedLocks = new List<IDistributedLock>();
            var lockKeys = createDto.AcademicClassIds.Select(id => $"enrollment:capacity:{id}").ToList();
            var lockExpiration = TimeSpan.FromMinutes(2);
            var lockTimeout = TimeSpan.FromSeconds(30);

            try
            {
                // Try to acquire locks for all classes
                foreach (var (academicClassId, index) in createDto.AcademicClassIds.Select((id, i) => (id, i)))
                {
                    var lockKey = lockKeys[index];
                    var distributedLock = await _distributedLockService.AcquireLockAsync(lockKey, lockExpiration, lockTimeout);
                    
                    if (distributedLock == null)
                    {
                        _logger.LogWarning("Failed to acquire lock for academic class {AcademicClassId}. Proceeding with database-level locking only.", academicClassId);
                    }
                    else
                    {
                        distributedLocks.Add(distributedLock);
                        _logger.LogDebug("Acquired distributed lock for academic class {AcademicClassId}", academicClassId);
                    }
                }

                // Start a single database transaction for ALL enrollments
                using var transaction = await _enrollmentRepository.BeginTransactionAsync();

                try
                {
                    var enrollmentsToCreate = new List<Enrollment>();
                    var validationResults = new List<(Guid AcademicClassId, dynamic AcademicClassData)>();

                    // Phase 1: Validate ALL academic classes and check capacity
                    foreach (var academicClassId in createDto.AcademicClassIds)
                    {
                        // Check if enrollment already exists
                        var existingEnrollment = await _enrollmentRepository.ExistsAsync(createDto.StudentId, academicClassId);
                        if (existingEnrollment)
                        {
                            _logger.LogWarning("Enrollment already exists for student {StudentId} and academic class {AcademicClassId}",
                                createDto.StudentId, academicClassId);
                            throw new InvalidOperationException($"Student {createDto.StudentId} is already enrolled in academic class {academicClassId}");
                        }

                        // Validate academic class exists and is registrable
                        var academicClassResponse = await _academicClassClient.GetAcademicClassById(academicClassId.ToString());
                        if (!academicClassResponse.Success || academicClassResponse.Data == null)
                        {
                            _logger.LogWarning("Academic class {AcademicClassId} not found", academicClassId);
                            throw new ArgumentException($"Academic class {academicClassId} not found");
                        }

                        var academicClass = academicClassResponse.Data;
                        if (!academicClass.IsRegistrable)
                        {
                            _logger.LogWarning("Academic class {AcademicClassId} is not registrable", academicClassId);
                            throw new InvalidOperationException($"Academic class {academicClassId} is not registrable");
                        }

                        // Check capacity with database lock to ensure consistency
                        var currentEnrollmentCount = await _enrollmentRepository.GetEnrollmentCountByAcademicClassIdWithLockAsync(academicClassId);

                        if (currentEnrollmentCount >= academicClass.Capacity)
                        {
                            _logger.LogWarning("Academic class {AcademicClassId} capacity exceeded. Current: {Current}, Capacity: {Capacity}",
                                academicClassId, currentEnrollmentCount, academicClass.Capacity);
                            throw new EnrollmentCapacityExceededException(academicClassId, currentEnrollmentCount, academicClass.Capacity);
                        }

                        validationResults.Add((academicClassId, academicClass));

                        // Create enrollment entity for later batch insertion
                        var enrollment = new Enrollment
                        {
                            StudentId = createDto.StudentId,
                            AcademicClassId = academicClassId,
                            Status = 1,
                            StudentResults = new List<StudentResult>()
                        };

                        enrollmentsToCreate.Add(enrollment);
                        _logger.LogDebug("Validated academic class {AcademicClassId} for enrollment", academicClassId);
                    }

                    // Phase 2: If all validations pass, create ALL enrollments in batch
                    var createdEnrollments = await _enrollmentRepository.CreateMultipleEnrollmentsWithoutTransactionAsync(enrollmentsToCreate);

                    // Phase 3: Commit transaction only if all enrollments were created successfully
                    await transaction.CommitAsync();

                    _logger.LogInformation("Successfully created {Count} enrollments for student {StudentId}", 
                        createdEnrollments.Count, createDto.StudentId);

                    // Phase 4: Populate DTOs for response
                    var result = new List<EnrollmentReadDto>();
                    foreach (var (enrollment, index) in createdEnrollments.Select((e, i) => (e, i)))
                    {
                        var (academicClassId, academicClassData) = validationResults[index];
                        var enrollmentDto = await PopulateEnrollmentDto(enrollment, studentResponse.Data, academicClassData);
                        result.Add(enrollmentDto);
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    // Rollback transaction on any error
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Rolling back all enrollments due to error for student {StudentId}", createDto.StudentId);
                    throw; // Re-throw the exception
                }
            }            finally
            {
                // Release all distributed locks
                foreach (var distributedLock in distributedLocks)
                {
                    distributedLock?.Dispose();
                }
                _logger.LogDebug("Released all distributed locks for student {StudentId}", createDto.StudentId);
            }
        }        
        private async Task<EnrollmentReadDto> PopulateEnrollmentDto(Enrollment enrollment, dynamic studentData, dynamic academicClassData)
        {
            var enrollmentDto = _mapper.Map<EnrollmentReadDto>(enrollment);
            
            // Populate student data
            var userData = studentData.User;
            enrollmentDto.Student = new GrpcStudentData
            {
                Id = Guid.Parse(studentData.Id),
                StudentCode = studentData.StudentCode,
                AccumulateCredits = studentData.AccumulateCredits,
                AccumulateScore = studentData.AccumulateScore,
                AccumulateActivityScore = studentData.AccumulateActivityScore,
                MajorId = !string.IsNullOrEmpty(studentData.MajorId) ? Guid.Parse(studentData.MajorId) : Guid.Empty,
                BatchId = !string.IsNullOrEmpty(studentData.BatchId) ? Guid.Parse(studentData.BatchId) : Guid.Empty,
                ApplicationUserId = !string.IsNullOrEmpty(studentData.ApplicationUserId) ? Guid.Parse(studentData.ApplicationUserId) : Guid.Empty,
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
            };            // Populate academic class data
            enrollmentDto.AcademicClass = new GrpcAcademicClassData
            {
                Id = Guid.Parse(academicClassData.Id),
                Name = academicClassData.Name,
                GroupNumber = academicClassData.GroupNumber,
                Capacity = academicClassData.Capacity,
                ListOfWeeks = academicClassData.ListOfWeeks != null ? new List<int>(academicClassData.ListOfWeeks) : null,
                IsRegistrable = academicClassData.IsRegistrable,
                SemesterId = !string.IsNullOrEmpty(academicClassData.SemesterId) ? Guid.Parse(academicClassData.SemesterId) : Guid.Empty,
                CourseId = !string.IsNullOrEmpty(academicClassData.CourseId) ? Guid.Parse(academicClassData.CourseId) : Guid.Empty,
                
                Course = academicClassData.Course != null ? new GrpcCourseData
                {
                    Id = Guid.Parse(academicClassData.Course.Id),
                    Code = academicClassData.Course.Code,
                    Name = academicClassData.Course.Name,
                    Description = academicClassData.Course.Description,
                    IsActive = academicClassData.Course.IsActive,
                    Credit = academicClassData.Course.Credit,
                    PracticePeriod = academicClassData.Course.PracticePeriod,
                    IsRequired = academicClassData.Course.IsRequired,
                    Cost = academicClassData.Course.Cost
                } : null
            };

            return enrollmentDto;
        }

        public async Task<int> GetEnrollmentCountByAcademicClassIdAsync(Guid academicClassId)
        {
            return await _enrollmentRepository.GetEnrollmentCountByAcademicClassIdAsync(academicClassId);
        }        
        public async Task<List<EnrollmentReadDto>> GetEnrollmentsByAcademicClassIdAsync(Guid academicClassId)
        {
            var enrollments = await _enrollmentRepository.GetEnrollmentsByAcademicClassIdAsync(academicClassId);
            var enrollmentDtos = _mapper.Map<List<EnrollmentReadDto>>(enrollments);

            // Get academic class data once since all enrollments are for the same class
            GrpcAcademicClassData? academicClassData = null;
            var academicClassResponse = await _academicClassClient.GetAcademicClassById(academicClassId.ToString());
            if (academicClassResponse.Success && academicClassResponse.Data != null)
            {
                var acClass = academicClassResponse.Data;
                
                academicClassData = new GrpcAcademicClassData
                {
                    Id = Guid.Parse(acClass.Id),
                    Name = acClass.Name,
                    GroupNumber = acClass.GroupNumber,
                    Capacity = acClass.Capacity,
                    ListOfWeeks = acClass.ListOfWeeks != null ? new List<int>(acClass.ListOfWeeks) : null,
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
                    } : null,                    // Map ScheduleInDays if available
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

            // Get student data for each enrollment
            foreach (var enrollmentDto in enrollmentDtos)
            {
                // Set academic class data (same for all enrollments)
                enrollmentDto.AcademicClass = academicClassData;

                // Get student data
                var enrollment = enrollments.FirstOrDefault(e => e.Id == enrollmentDto.Id);
                if (enrollment != null)
                {
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
                }
            }

            return enrollmentDtos;
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
        public async Task<List<EnrollmentReadDto>> GetEnrollmentsByStudentIdAsync(Guid studentId, Guid? semesterId = null)
        {
            var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentIdAsync(studentId, semesterId);
            var enrollmentDtos = _mapper.Map<List<EnrollmentReadDto>>(enrollments);
            var filteredEnrollmentDtos = new List<EnrollmentReadDto>();

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
                    
                    // Filter by semesterId if provided
                    if (semesterId.HasValue)
                    {
                        var academicClassSemesterId = !string.IsNullOrEmpty(acClass.SemesterId) ? Guid.Parse(acClass.SemesterId) : Guid.Empty;
                        if (academicClassSemesterId != semesterId.Value)
                        {
                            continue; // Skip this enrollment as it doesn't match the semester filter
                        }
                    }
                    
                    enrollmentDto.AcademicClass = new GrpcAcademicClassData
                    {
                        Id = Guid.Parse(acClass.Id),
                        Name = acClass.Name,
                        GroupNumber = acClass.GroupNumber,
                        Capacity = acClass.Capacity,
                        ListOfWeeks = acClass.ListOfWeeks != null ? new List<int>(acClass.ListOfWeeks) : null,
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
                    
                    // Add to filtered list if academic class data was populated successfully
                    filteredEnrollmentDtos.Add(enrollmentDto);
                }
                else if (!semesterId.HasValue)
                {
                    // If no semester filter and academic class data couldn't be retrieved, 
                    // still include the enrollment but without academic class data
                    filteredEnrollmentDtos.Add(enrollmentDto);
                }
            }

            return filteredEnrollmentDtos;
        }

        public async Task<bool> DeleteEnrollmentAsync(Guid id)
        {
            try
            {
                var enrollment = await _enrollmentRepository.GetEnrollmentByIdAsync(id);
                if (enrollment == null)
                {
                    _logger.LogWarning("Enrollment with ID {EnrollmentId} not found for deletion", id);
                    return false;
                }

                _logger.LogInformation("Deleting enrollment with ID {EnrollmentId} for Student {StudentId} and AcademicClass {AcademicClassId}", 
                    id, enrollment.StudentId, enrollment.AcademicClassId);

                var result = await _enrollmentRepository.DeleteEnrollmentAsync(id);
                
                if (result)
                {
                    _logger.LogInformation("Successfully deleted enrollment with ID {EnrollmentId}", id);
                }
                else
                {
                    _logger.LogWarning("Failed to delete enrollment with ID {EnrollmentId}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting enrollment with ID {EnrollmentId}", id);
                throw;
            }
        }

        public async Task<int> ApproveEnrollmentsByAcademicClassIdAsync(Guid classId)
        {
            try
            {
                _logger.LogInformation("Approving enrollments for Academic Class ID {ClassId}", classId);

                var result = await _enrollmentRepository.ApproveEnrollmentsByAcademicClassIdAsync(classId);
                
                _logger.LogInformation("Successfully approved {Count} enrollments for Academic Class ID {ClassId}", result, classId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while approving enrollments for Academic Class ID {ClassId}", classId);
                throw;
            }
        }
        public async Task<int> RejectEnrollmentsByAcademicClassIdAsync(Guid classId)
        {
            try
            {
                var result = await _enrollmentRepository.RejectEnrollmentsByAcademicClassIdAsync(classId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while rejecting enrollments for Academic Class ID {ClassId}", classId);
                throw;
            }
        }
        public async Task<int> MoveEnrollmentsToNewClassAsync(List<Guid> enrollmentIds, Guid toClassId)
        {
            try
            {
                var result = await _enrollmentRepository.MoveEnrollmentsToNewClassAsync(enrollmentIds, toClassId);
                _logger.LogInformation("Successfully moved {Count} enrollments to class {ToClassId}",
                    result, toClassId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while moving {Count} enrollments to class {ToClassId}",
                    enrollmentIds.Count, toClassId);
                throw;
            }
        }        public async Task<int?> GetFirstEnrollmentStatusByAcademicClassIdAsync(Guid academicClassId)
        {
            return await _enrollmentRepository.GetFirstEnrollmentStatusByAcademicClassIdAsync(academicClassId);
        }        
        public async Task<CheckClassConflictResponse> CheckClassConflictAsync(CheckClassConflictRequest request)
        {
            try
            {
                _logger.LogInformation("Checking class conflicts for class {ClassId} with {EnrollmentCount} enrollments",
                    request.ClassToCheckId, request.EnrollmentIds.Count);

                var response = new CheckClassConflictResponse();

                // Get the class to check schedule (if provided)
                GrpcAcademicClassData? classToCheck = null;
                if (request.ClassToCheckId.HasValue)
                {
                    var classToCheckResponse = await _academicClassClient.GetAcademicClassById(request.ClassToCheckId.Value.ToString());
                    if (!classToCheckResponse.Success || classToCheckResponse.Data == null)
                    {
                        _logger.LogWarning("Class to check with ID {ClassId} not found", request.ClassToCheckId);
                        // Continue processing but all enrollments will have IsConflict = false
                    }
                    else
                    {
                        var classData = classToCheckResponse.Data;
                        classToCheck = new GrpcAcademicClassData
                        {
                            Id = Guid.Parse(classData.Id),
                            Name = classData.Name,
                            GroupNumber = classData.GroupNumber,
                            Capacity = classData.Capacity,
                            ListOfWeeks = classData.ListOfWeeks?.ToList(),
                            IsRegistrable = classData.IsRegistrable,
                            SemesterId = !string.IsNullOrEmpty(classData.SemesterId) ? Guid.Parse(classData.SemesterId) : Guid.Empty,
                            CourseId = !string.IsNullOrEmpty(classData.CourseId) ? Guid.Parse(classData.CourseId) : Guid.Empty,
                            ScheduleInDays = classData.ScheduleInDays?.Select(s => new GrpcScheduleInDayData
                            {
                                Id = !string.IsNullOrEmpty(s.Id) ? Guid.Parse(s.Id) : Guid.Empty,
                                DayOfWeek = s.DayOfWeek,
                                RoomId = !string.IsNullOrEmpty(s.RoomId) ? Guid.Parse(s.RoomId) : Guid.Empty,
                                ShiftId = !string.IsNullOrEmpty(s.ShiftId) ? Guid.Parse(s.ShiftId) : Guid.Empty,
                                Shift = s.Shift != null ? new GrpcShiftData
                                {
                                    Id = !string.IsNullOrEmpty(s.Shift.Id) ? Guid.Parse(s.Shift.Id) : Guid.Empty,
                                    Name = s.Shift.Name,
                                    StartTime = s.Shift.StartTime,
                                    EndTime = s.Shift.EndTime
                                } : null
                            }).ToList()
                        };
                    }
                }

                // Get all enrollments with their academic class data
                var enrollments = await _enrollmentRepository.GetEnrollmentsByIdsAsync(request.EnrollmentIds);
                
                foreach (var enrollment in enrollments)
                {
                    var enrollmentDto = _mapper.Map<EnrollmentReadDto>(enrollment);
                    
                    // Get student data
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

                    // Create conflict check DTO first
                    var conflictDto = new EnrollmentConflictCheckDto
                    {
                        Id = enrollmentDto.Id,
                        Status = enrollmentDto.Status,
                        StudentId = enrollmentDto.StudentId,
                        AcademicClassId = enrollmentDto.AcademicClassId,
                        CreatedAt = enrollmentDto.CreatedAt,
                        UpdatedAt = enrollmentDto.UpdatedAt,
                        Student = enrollmentDto.Student,
                        AcademicClass = null, // Will be set later if needed
                        IsConflict = false
                    };

                    // Check for conflicts if classToCheck is provided
                    if (classToCheck != null)
                    {
                        // Get all enrollments of this student to check for conflicts
                        var studentEnrollments = await _enrollmentRepository.GetEnrollmentsByStudentIdAsync(enrollment.StudentId);
                        
                        // Check each of the student's enrolled classes against the classToCheck
                        foreach (var studentEnrollment in studentEnrollments)
                        {
                            // Skip the current enrollment being checked
                            if (studentEnrollment.Id == enrollment.Id)
                                continue;

                            // Get the academic class data for this student's enrollment
                            var studentClassResponse = await _academicClassClient.GetAcademicClassById(studentEnrollment.AcademicClassId.ToString());
                            if (studentClassResponse.Success && studentClassResponse.Data != null)
                            {
                                var studentClassData = studentClassResponse.Data;                                var studentClass = new GrpcAcademicClassData
                                {
                                    Id = Guid.Parse(studentClassData.Id),
                                    ListOfWeeks = studentClassData.ListOfWeeks?.ToList(),
                                    ScheduleInDays = studentClassData.ScheduleInDays?.Select(s => new GrpcScheduleInDayData
                                    {
                                        Id = !string.IsNullOrEmpty(s.Id) ? Guid.Parse(s.Id) : Guid.Empty,
                                        DayOfWeek = s.DayOfWeek,
                                        ShiftId = !string.IsNullOrEmpty(s.ShiftId) ? Guid.Parse(s.ShiftId) : Guid.Empty,
                                        Shift = s.Shift != null ? new GrpcShiftData
                                        {
                                            Id = !string.IsNullOrEmpty(s.Shift.Id) ? Guid.Parse(s.Shift.Id) : Guid.Empty,
                                            Name = s.Shift.Name,
                                            StartTime = s.Shift.StartTime,
                                            EndTime = s.Shift.EndTime
                                        } : null
                                    }).ToList()
                                };                                // Check for conflict between classToCheck and this student's class
                                if (classToCheck.ScheduleInDays != null && 
                                    studentClass.ScheduleInDays != null &&
                                    classToCheck.ListOfWeeks != null &&
                                    studentClass.ListOfWeeks != null)
                                {
                                    if (HasScheduleConflict(
                                        classToCheck.ScheduleInDays, 
                                        studentClass.ScheduleInDays,
                                        classToCheck.ListOfWeeks,
                                        studentClass.ListOfWeeks))
                                    {
                                        conflictDto.IsConflict = true;
                                        break; // Found conflict, no need to check other classes
                                    }
                                }
                            }
                        }
                    }

                    response.Enrollments.Add(conflictDto);
                }

                _logger.LogInformation("Conflict check completed for {TotalCount} enrollments",
                    response.Enrollments.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking class conflicts for class {ClassId}", 
                    request.ClassToCheckId);
                throw;
            }
        }        private bool HasScheduleConflict(List<GrpcScheduleInDayData> schedule1, List<GrpcScheduleInDayData> schedule2, List<int> weeks1, List<int> weeks2)
        {
            // First condition: Check if the weeks have intersection
            if (!weeks1.Intersect(weeks2).Any())
            {
                return false; // No conflict if no week intersection
            }

            // Second condition: Check if they have the same day of week AND overlapping shifts
            foreach (var slot1 in schedule1)
            {
                foreach (var slot2 in schedule2)
                {
                    // Check if same day of week and overlapping shifts
                    if (slot1.DayOfWeek?.Equals(slot2.DayOfWeek, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        if (HasShiftConflict(slot1, slot2))
                        {
                            return true; // Conflict found
                        }
                    }
                }
            }
            
            return false; // No conflict
        }

        private bool HasShiftConflict(GrpcScheduleInDayData slot1, GrpcScheduleInDayData slot2)
        {
            // If exact same shift ID, definitely a conflict
            if (slot1.ShiftId == slot2.ShiftId)
            {
                return true;
            }

            // If both slots have shift data, check for time overlaps and pattern conflicts
            if (slot1.Shift != null && slot2.Shift != null)
            {
                // Check for time overlap
                if (HasTimeOverlap(slot1.Shift.StartTime, slot1.Shift.EndTime, slot2.Shift.StartTime, slot2.Shift.EndTime))
                {
                    return true;
                }

                // Check for pattern-based conflicts (e.g., "morning full" vs "morning 1", "morning 2")
                if (HasShiftPatternConflict(slot1.Shift.Name, slot2.Shift.Name))
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasTimeOverlap(string startTime1, string endTime1, string startTime2, string endTime2)
        {
            // Parse time strings (assuming format like "08:00" or "08:00:00")
            if (!TimeSpan.TryParse(startTime1, out var start1) ||
                !TimeSpan.TryParse(endTime1, out var end1) ||
                !TimeSpan.TryParse(startTime2, out var start2) ||
                !TimeSpan.TryParse(endTime2, out var end2))
            {
                // If we can't parse times, assume no conflict to be safe
                return false;
            }

            // Check if time ranges overlap
            // Two time ranges overlap if: start1 < end2 AND start2 < end1
            return start1 < end2 && start2 < end1;
        }

        private bool HasShiftPatternConflict(string shiftName1, string shiftName2)
        {
            if (string.IsNullOrEmpty(shiftName1) || string.IsNullOrEmpty(shiftName2))
                return false;

            var name1 = shiftName1.ToLowerInvariant().Trim();
            var name2 = shiftName2.ToLowerInvariant().Trim();

            // Check for morning patterns
            if (IsMorningShift(name1) && IsMorningShift(name2))
            {
                // If one is "morning full" and the other is any morning shift, they conflict
                if (name1.Contains("full") || name2.Contains("full"))
                {
                    return true;
                }
            }

            // Check for afternoon patterns
            if (IsAfternoonShift(name1) && IsAfternoonShift(name2))
            {
                // If one is "afternoon full" and the other is any afternoon shift, they conflict
                if (name1.Contains("full") || name2.Contains("full"))
                {
                    return true;
                }
            }

            // Check for evening patterns
            if (IsEveningShift(name1) && IsEveningShift(name2))
            {
                // If one is "evening full" and the other is any evening shift, they conflict
                if (name1.Contains("full") || name2.Contains("full"))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsMorningShift(string shiftName)
        {
            return shiftName.Contains("morning") || shiftName.Contains("sáng");
        }

        private bool IsAfternoonShift(string shiftName)
        {
            return shiftName.Contains("afternoon") || shiftName.Contains("chiều");
        }

        private bool IsEveningShift(string shiftName)
        {
            return shiftName.Contains("evening") || shiftName.Contains("tối");
        }}
}
