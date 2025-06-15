using AutoMapper;
using CourseService.Business.Dtos.AcademicClass;
using CourseService.CommunicationTypes.Grpc.GrpcClient;
using CourseService.DataAccess.Repositories;
using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;
using MajorService;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Business.Services
{    public class AcademicClassService : IAcademicClassService
    {        
        private readonly IAcademicClassRepository _academicClassRepository;
        private readonly IScheduleInDayRepository _scheduleInDayRepository;
        private readonly IMapper _mapper;
        private readonly GrpcRoomClientService _roomClientService;
        private readonly GrpcEnrollmentClientService _enrollmentClientService;
        private readonly ILogger<AcademicClassService> _logger;

        public AcademicClassService(
            IAcademicClassRepository academicClassRepository,
            IScheduleInDayRepository scheduleInDayRepository,
            GrpcRoomClientService roomClientService,
            GrpcEnrollmentClientService enrollmentClientService,
            IMapper mapper,
            ILogger<AcademicClassService> logger)
        {            _academicClassRepository = academicClassRepository;
            _scheduleInDayRepository = scheduleInDayRepository;
            _roomClientService = roomClientService;
            _enrollmentClientService = enrollmentClientService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<AcademicClassReadDto> CreateAcademicClassAsync(AcademicClassCreateDto academicClassCreateDto)
        {
            // Validate schedule conflicts before creating the academic class
            await ValidateScheduleConflictsAsync(academicClassCreateDto);

            // Create only the AcademicClass first without schedules
            var academicClass = new AcademicClass
            {
                Name = academicClassCreateDto.Name,
                GroupNumber = academicClassCreateDto.GroupNumber,
                Capacity = academicClassCreateDto.Capacity,
                ListOfWeeks = academicClassCreateDto.ListOfWeeks,
                IsRegistrable = academicClassCreateDto.IsRegistrable,
                CourseId = academicClassCreateDto.CourseId,
                SemesterId = academicClassCreateDto.SemesterId,
                ParentTheoryAcademicClassId = academicClassCreateDto.ParentTheoryAcademicClassId,
                // Initialize with empty lists
                ChildPracticeAcademicClassIds = new List<Guid>(),
                ScheduleInDays = new List<ScheduleInDay>()
            };

            // Create and save the AcademicClass first to get its Id
            var createdAcademicClass = await _academicClassRepository.CreateAcademicClassAsync(academicClass);

            // If this is a practice class (has a parent), update the parent's ChildPracticeAcademicClassIds
            if (academicClassCreateDto.ParentTheoryAcademicClassId.HasValue)
            {
                var parentClass = await _academicClassRepository.GetAcademicClassByIdAsync(academicClassCreateDto.ParentTheoryAcademicClassId.Value);
                if (parentClass != null)
                {
                    // Add this practice class ID to parent's child list
                    if (parentClass.ChildPracticeAcademicClassIds == null)
                    {
                        parentClass.ChildPracticeAcademicClassIds = new List<Guid>();
                    }
                    parentClass.ChildPracticeAcademicClassIds.Add(createdAcademicClass.Id);

                    // Update the parent class
                    await _academicClassRepository.UpdateAcademicClassAsync(parentClass);
                }
            }

            // Create ScheduleInDays separately
            foreach (var scheduleDto in academicClassCreateDto.ScheduleInDays)
            {
                var scheduleInDay = new ScheduleInDay
                {
                    DayOfWeek = scheduleDto.DayOfWeek,
                    RoomId = scheduleDto.RoomId,
                    ShiftId = scheduleDto.ShiftId,
                    AcademicClassId = createdAcademicClass.Id
                };

                // Save each ScheduleInDay
                await _scheduleInDayRepository.CreateScheduleInDayAsync(scheduleInDay);
            }

            // Get the complete AcademicClass with its relationships
            var result = await _academicClassRepository.GetAcademicClassByIdAsync(createdAcademicClass.Id);

            var academicClassDto = _mapper.Map<AcademicClassReadDto>(result);

            // The schedules are already included in academicClassDto through the mapping
            // We only need to populate the room data
            if (academicClassDto.ScheduleInDays != null && academicClassDto.ScheduleInDays.Count > 0)
            {
                foreach (var schedule in academicClassDto.ScheduleInDays)
                {
                    try
                    {
                        var roomResponse = await _roomClientService.GetRoomByIdAsync(schedule.RoomId.ToString());
                        if (roomResponse != null && roomResponse.Success)
                        {
                            schedule.Room = roomResponse.Data;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error but continue with other schedules
                        Console.WriteLine($"Error fetching room data: {ex.Message}");
                    }
                }
            }

            return academicClassDto;
        }
        public async Task<AcademicClassReadDto> GetAcademicClassByIdAsync(Guid id)
        {
            var academicClass = await _academicClassRepository.GetAcademicClassByIdAsync(id);

            if (academicClass == null)
            {
                throw new Exception($"Academic class with ID {id} not found");
            }
            var academicClassDto = _mapper.Map<AcademicClassReadDto>(academicClass);

            // Filter out child practice classes where IsRegistrable = false
            academicClassDto.ChildPracticeAcademicClasses = academicClassDto.ChildPracticeAcademicClasses
                .Where(child => child.IsRegistrable)
                .ToList();

            // Populate room data for the main academic class schedules
            if (academicClassDto.ScheduleInDays != null && academicClassDto.ScheduleInDays.Count > 0)
            {
                foreach (var schedule in academicClassDto.ScheduleInDays)
                {
                    try
                    {
                        var roomResponse = await _roomClientService.GetRoomByIdAsync(schedule.RoomId.ToString());
                        if (roomResponse != null && roomResponse.Success)
                        {
                            schedule.Room = roomResponse.Data;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error but continue with other schedules
                        Console.WriteLine($"Error fetching room data: {ex.Message}");
                    }
                }
            }

            // Populate room data for child practice classes
            if (academicClassDto.ChildPracticeAcademicClasses != null && academicClassDto.ChildPracticeAcademicClasses.Count > 0)
            {
                foreach (var childClass in academicClassDto.ChildPracticeAcademicClasses)
                {
                    if (childClass.ScheduleInDays != null && childClass.ScheduleInDays.Count > 0)
                    {
                        foreach (var schedule in childClass.ScheduleInDays)
                        {
                            try
                            {
                                var roomResponse = await _roomClientService.GetRoomByIdAsync(schedule.RoomId.ToString());
                                if (roomResponse != null && roomResponse.Success)
                                {
                                    schedule.Room = roomResponse.Data;
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log the error but continue with other schedules
                                Console.WriteLine($"Error fetching room data for child class {childClass.Id}: {ex.Message}");
                            }
                        }
                    }
                }
            }

            // Get enrollment count for the main academic class
            try
            {
                var enrollmentCountResponse = await _enrollmentClientService.GetEnrollmentCountAsync(id.ToString());
                if (enrollmentCountResponse != null && enrollmentCountResponse.Success)
                {
                    academicClassDto.EnrollmentCount = enrollmentCountResponse.Count;
                }
                else
                {
                    academicClassDto.EnrollmentCount = 0;
                }
            }
            catch (Exception ex)
            {
                // Log the error but continue with enrollment count as 0
                _logger.LogError(ex, "Error fetching enrollment count for academic class {AcademicClassId}", id);
                academicClassDto.EnrollmentCount = 0;
            }

            // Get enrollment status for the main academic class
            try
            {
                var enrollmentStatus = await _enrollmentClientService.GetFirstEnrollmentStatusAsync(id.ToString());
                academicClassDto.EnrollmentStatus = enrollmentStatus;
            }
            catch (Exception ex)
            {
                // Log the error but continue with null enrollment status
                _logger.LogError(ex, "Error fetching enrollment status for academic class {AcademicClassId}", id);
                academicClassDto.EnrollmentStatus = null;
            }

            // Get enrollment count and status for child practice classes
            if (academicClassDto.ChildPracticeAcademicClasses != null && academicClassDto.ChildPracticeAcademicClasses.Count > 0)
            {
                foreach (var childClass in academicClassDto.ChildPracticeAcademicClasses)
                {
                    try
                    {
                        var enrollmentCountResponse = await _enrollmentClientService.GetEnrollmentCountAsync(childClass.Id.ToString());
                        if (enrollmentCountResponse != null && enrollmentCountResponse.Success)
                        {
                            childClass.EnrollmentCount = enrollmentCountResponse.Count;
                        }
                        else
                        {
                            childClass.EnrollmentCount = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error but continue with enrollment count as 0
                        _logger.LogError(ex, "Error fetching enrollment count for child academic class {ChildAcademicClassId}", childClass.Id);
                        childClass.EnrollmentCount = 0;
                    }

                    try
                    {
                        var enrollmentStatus = await _enrollmentClientService.GetFirstEnrollmentStatusAsync(childClass.Id.ToString());
                        childClass.EnrollmentStatus = enrollmentStatus;
                    }
                    catch (Exception ex)
                    {
                        // Log the error but continue with null enrollment status
                        _logger.LogError(ex, "Error fetching enrollment status for child academic class {ChildAcademicClassId}", childClass.Id);
                        childClass.EnrollmentStatus = null;
                    }
                }
            }

            return academicClassDto;
        }        
        public async Task<List<AcademicClassReadDto>> GetAcademicClassesByCourseIdAsync(Guid courseId)
        {
            var academicClasses = await _academicClassRepository.GetAcademicClassesByCourseIdAsync(courseId);
            var academicClassDtos = _mapper.Map<List<AcademicClassReadDto>>(academicClasses);

            // Filter out child practice classes where IsRegistrable = false
            foreach (var academicClass in academicClassDtos)
            {
                academicClass.ChildPracticeAcademicClasses = academicClass.ChildPracticeAcademicClasses
                    .Where(child => child.IsRegistrable)
                    .ToList();
            }

            // Populate room data for each academic class
            await PopulateRoomDataForClasses(academicClassDtos);

            // Populate enrollment count for each academic class
            await PopulateEnrollmentCountForClasses(academicClassDtos);

            return academicClassDtos;
        }        
        public async Task<List<AcademicClassReadDto>> GetAcademicClassesBySemesterIdAsync(Guid semesterId)
        {
            var academicClasses = await _academicClassRepository.GetAcademicClassesBySemesterIdAsync(semesterId);
            var academicClassDtos = _mapper.Map<List<AcademicClassReadDto>>(academicClasses);

            // Filter out child practice classes where IsRegistrable = false
            foreach (var academicClass in academicClassDtos)
            {
                academicClass.ChildPracticeAcademicClasses = academicClass.ChildPracticeAcademicClasses
                    .Where(child => child.IsRegistrable)
                    .ToList();
            }

            // Populate room data for each academic class
            await PopulateRoomDataForClasses(academicClassDtos);

            // Populate enrollment count for each academic class
            await PopulateEnrollmentCountForClasses(academicClassDtos);

            return academicClassDtos;
        }
        public async Task<List<AcademicClassReadDto>> GetAcademicClassesForMajorAsync(Guid majorId)
        {
            var academicClasses = await _academicClassRepository.GetAcademicClassesForMajorAsync(majorId);
            var academicClassDtos = _mapper.Map<List<AcademicClassReadDto>>(academicClasses);

            // Filter out child practice classes where IsRegistrable = false
            foreach (var academicClass in academicClassDtos)
            {
                academicClass.ChildPracticeAcademicClasses = academicClass.ChildPracticeAcademicClasses
                    .Where(child => child.IsRegistrable)
                    .ToList();
            }

            // Populate room data for each academic class
            await PopulateRoomDataForClasses(academicClassDtos);

            // Populate enrollment count for each academic class
            await PopulateEnrollmentCountForClasses(academicClassDtos);

            return academicClassDtos;
        }          
        public async Task<List<AcademicClassReadDto>> GetAcademicClassesForMajorAndBatchAsync(Guid majorId, Guid batchId)
        {
            var academicClasses = await _academicClassRepository.GetAcademicClassesForMajorAndBatchAsync(majorId, batchId);
            var academicClassDtos = _mapper.Map<List<AcademicClassReadDto>>(academicClasses);
            
            // Filter out child practice classes where IsRegistrable = false
            foreach (var academicClass in academicClassDtos)
            {
                academicClass.ChildPracticeAcademicClasses = academicClass.ChildPracticeAcademicClasses
                    .Where(child => child.IsRegistrable)
                    .ToList();
            }
            
            // Populate room data for each academic class
            await PopulateRoomDataForClasses(academicClassDtos);
            
            // Populate enrollment count for each academic class
            await PopulateEnrollmentCountForClasses(academicClassDtos);
            
            return academicClassDtos;
        }
        
        public async Task<List<AcademicClassReadDto>> GetAcademicClassesBySemesterAndCourseIdAsync(Guid semesterId, Guid courseId)
        {
            var academicClasses = await _academicClassRepository.GetAcademicClassesBySemesterAndCourseIdAsync(semesterId, courseId);
            var academicClassDtos = _mapper.Map<List<AcademicClassReadDto>>(academicClasses);

            // Filter out child practice classes where IsRegistrable = false
            foreach (var academicClass in academicClassDtos)
            {
                academicClass.ChildPracticeAcademicClasses = academicClass.ChildPracticeAcademicClasses
                    .Where(child => child.IsRegistrable)
                    .ToList();
            }

            // Populate room data for each academic class
            await PopulateRoomDataForClasses(academicClassDtos);

            // Populate enrollment count for each academic class
            await PopulateEnrollmentCountForClasses(academicClassDtos);

            return academicClassDtos;
        }

        // Helper method to populate room data for a list of academic classes
        private async Task PopulateRoomDataForClasses(List<AcademicClassReadDto> academicClasses)
        {
            foreach (var academicClass in academicClasses)
            {
                // Populate room data for the main academic class schedules
                if (academicClass.ScheduleInDays != null && academicClass.ScheduleInDays.Count > 0)
                {
                    foreach (var schedule in academicClass.ScheduleInDays)
                    {
                        try
                        {
                            var roomResponse = await _roomClientService.GetRoomByIdAsync(schedule.RoomId.ToString());
                            if (roomResponse != null && roomResponse.Success)
                            {
                                schedule.Room = roomResponse.Data;
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log the error but continue with other schedules
                            Console.WriteLine($"Error fetching room data: {ex.Message}");
                        }
                    }
                }

                // Populate room data for child practice classes
                if (academicClass.ChildPracticeAcademicClasses != null && academicClass.ChildPracticeAcademicClasses.Count > 0)
                {
                    foreach (var childClass in academicClass.ChildPracticeAcademicClasses)
                    {
                        if (childClass.ScheduleInDays != null && childClass.ScheduleInDays.Count > 0)
                        {
                            foreach (var schedule in childClass.ScheduleInDays)
                            {
                                try
                                {
                                    var roomResponse = await _roomClientService.GetRoomByIdAsync(schedule.RoomId.ToString());
                                    if (roomResponse != null && roomResponse.Success)
                                    {
                                        schedule.Room = roomResponse.Data;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // Log the error but continue with other schedules
                                    Console.WriteLine($"Error fetching room data for child class {childClass.Id}: {ex.Message}");
                                }
                            }
                        }
                    }
                }
            }
        }        
        private async Task PopulateEnrollmentCountForClasses(List<AcademicClassReadDto> academicClasses)
        {
            foreach (var academicClass in academicClasses)
            {
                // Populate enrollment count for the main academic class
                try
                {
                    var enrollmentCountResponse = await _enrollmentClientService.GetEnrollmentCountAsync(academicClass.Id.ToString());
                    if (enrollmentCountResponse != null && enrollmentCountResponse.Success)
                    {
                        academicClass.EnrollmentCount = enrollmentCountResponse.Count;
                    }
                    else
                    {
                        academicClass.EnrollmentCount = 0;
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but continue with enrollment count as 0
                    _logger.LogError(ex, "Error fetching enrollment count for academic class {AcademicClassId}", academicClass.Id);
                    academicClass.EnrollmentCount = 0;
                }

                // Populate enrollment status for the main academic class
                try
                {
                   var enrollmentStatus = await _enrollmentClientService.GetFirstEnrollmentStatusAsync(academicClass.Id.ToString());
                   academicClass.EnrollmentStatus = enrollmentStatus;
                }
                catch (Exception ex)
                {
                   // Log the error but continue with null enrollment status
                   _logger.LogError(ex, "Error fetching enrollment status for academic class {AcademicClassId}", academicClass.Id);
                   academicClass.EnrollmentStatus = null;
                }

                // Populate enrollment count and status for child practice classes
                if (academicClass.ChildPracticeAcademicClasses != null && academicClass.ChildPracticeAcademicClasses.Count > 0)
                {
                    foreach (var childClass in academicClass.ChildPracticeAcademicClasses)
                    {
                        try
                        {
                            var enrollmentCountResponse = await _enrollmentClientService.GetEnrollmentCountAsync(childClass.Id.ToString());
                            if (enrollmentCountResponse != null && enrollmentCountResponse.Success)
                            {
                                childClass.EnrollmentCount = enrollmentCountResponse.Count;
                            }
                            else
                            {
                                childClass.EnrollmentCount = 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log the error but continue with enrollment count as 0
                            _logger.LogError(ex, "Error fetching enrollment count for child academic class {ChildAcademicClassId}", childClass.Id);
                            childClass.EnrollmentCount = 0;
                        }

                        try
                        {
                           var enrollmentStatus = await _enrollmentClientService.GetFirstEnrollmentStatusAsync(childClass.Id.ToString());
                           childClass.EnrollmentStatus = enrollmentStatus;
                        }
                        catch (Exception ex)
                        {
                           // Log the error but continue with null enrollment status
                           _logger.LogError(ex, "Error fetching enrollment status for child academic class {ChildAcademicClassId}", childClass.Id);
                           childClass.EnrollmentStatus = null;
                        }
                    }
                }
            }
        }
        public async Task<AcademicClassListResponse> GetAllAcademicClassesPaginationAsync(
            Pagination pagination,
            AcademicClassFilterParams? filterParams,
            Order? order)
        {            
            // If enrollment status filtering is requested, we need to get all data first
            // then filter and paginate manually to ensure correct results
            if (filterParams?.EnrollmentStatus.HasValue == true)
            {
                // Get all academic classes with includes using repository method
                var allClassesQuery = _academicClassRepository.GetQueryWithIncludes();

                // Apply basic filters (without enrollment status) using repository method
                var tempFilterParams = new AcademicClassFilterParams
                {
                    Name = filterParams.Name,
                    GroupNumber = filterParams.GroupNumber,
                    MinCapacity = filterParams.MinCapacity,
                    MaxCapacity = filterParams.MaxCapacity,
                    StartDate = filterParams.StartDate,
                    EndDate = filterParams.EndDate,
                    IsRegistrable = filterParams.IsRegistrable,
                    CourseId = filterParams.CourseId,
                    SemesterId = filterParams.SemesterId,
                    RoomId = filterParams.RoomId,
                    ShiftId = filterParams.ShiftId,
                    ScheduleInDayIds = filterParams.ScheduleInDayIds
                    // Note: Exclude EnrollmentStatus from basic filtering
                };

                // Apply filters and sorting using repository methods
                var filteredQuery = _academicClassRepository.ApplyFiltersToQuery(allClassesQuery, tempFilterParams);

                // Apply sorting if specified
                if (order != null && !string.IsNullOrEmpty(order.By))
                {
                    if (order.IsDesc)
                    {
                        filteredQuery = filteredQuery.OrderByDescending(e => EF.Property<object>(e, order.By));
                    }
                    else
                    {
                        filteredQuery = filteredQuery.OrderBy(e => EF.Property<object>(e, order.By));
                    }
                }
                else
                {
                    filteredQuery = filteredQuery.OrderByDescending(s => s.CreatedAt);
                }

                // Execute query to get all filtered results
                var allFilteredClasses = await filteredQuery.ToListAsync();
                var allAcademicClassDtos = _mapper.Map<List<AcademicClassReadDto>>(allFilteredClasses);

                // Filter out child practice classes where IsRegistrable = false
                foreach (var academicClass in allAcademicClassDtos)
                {
                    academicClass.ChildPracticeAcademicClasses = academicClass.ChildPracticeAcademicClasses
                        .Where(child => child.IsRegistrable)
                        .ToList();
                }

                // Now filter by enrollment status via gRPC calls
                var filteredByEnrollmentStatus = new List<AcademicClassReadDto>();

                foreach (var academicClass in allAcademicClassDtos)
                {
                    try
                    {
                        // Get the first enrollment status for this academic class via gRPC
                        var enrollmentStatus = await _enrollmentClientService.GetFirstEnrollmentStatusAsync(academicClass.Id.ToString());

                        // Map the actual enrollment status to the filter status
                        // Filter status: 1=pending, 2=approved, 3=started (includes 3,4,5), 6=rejected
                        int? mappedStatus = null;
                        if (enrollmentStatus != 0)
                        {
                            switch (enrollmentStatus)
                            {
                                case 1:
                                    mappedStatus = 1; // pending
                                    break;
                                case 2:
                                    mappedStatus = 2; // approved
                                    break;
                                case 3:
                                case 4:
                                case 5:
                                    mappedStatus = 3; // started (includes 3, 4, 5)
                                    break;
                                case 6:
                                    mappedStatus = 6; // rejected
                                    break;
                                default:
                                    mappedStatus = 0; // unknown status
                                    break;
                            }
                        }

                        // If mapped enrollment status matches the filter criteria, include the class
                        if (mappedStatus.HasValue && mappedStatus.Value == filterParams.EnrollmentStatus.Value)
                        {
                            filteredByEnrollmentStatus.Add(academicClass);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error but continue processing other classes
                        _logger.LogWarning(ex, "Failed to get enrollment status for academic class {ClassId}", academicClass.Id);

                        // Optionally include the class if gRPC call fails (depending on business logic)
                        // For now, we'll exclude it from results when gRPC fails
                    }
                }

                // Apply manual pagination on the filtered results
                var totalCount = filteredByEnrollmentStatus.Count;
                var pagedResults = filteredByEnrollmentStatus
                    .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                    .Take(pagination.ItemsPerpage)
                    .ToList();

                // Populate room data for each academic class
                await PopulateRoomDataForClasses(pagedResults);

                // Populate enrollment count for each academic class
                await PopulateEnrollmentCountForClasses(pagedResults);

                return new AcademicClassListResponse
                {
                    Data = pagedResults,
                    Total = totalCount,
                    PageSize = pagination.ItemsPerpage,
                    PageIndex = pagination.PageNumber
                };
            }
            else
            {
                // Normal pagination without enrollment status filtering
                var paginationResult = await _academicClassRepository.GetAllAcademicClassesPaginationAsync(pagination, filterParams, order);
                var academicClassDtos = _mapper.Map<List<AcademicClassReadDto>>(paginationResult.Data);

                // Populate room data for each academic class
                await PopulateRoomDataForClasses(academicClassDtos);

                // Populate enrollment count for each academic class
                await PopulateEnrollmentCountForClasses(academicClassDtos);

                return new AcademicClassListResponse
                {
                    Data = academicClassDtos,
                    Total = paginationResult.Total,
                    PageSize = paginationResult.PageSize,
                    PageIndex = paginationResult.PageIndex
                };
            }
        }

        /// <summary>
        /// Schedule registration times for multiple academic classes
        /// </summary>
        public async Task<bool> ScheduleRegistrationAsync(ClassRegistrationScheduleDto scheduleDto)
        {
            if (scheduleDto.AcademicClassIds == null || !scheduleDto.AcademicClassIds.Any())
            {
                _logger.LogWarning("No academic class IDs provided for scheduling registration");
                return false;
            }

            var classes = await _academicClassRepository.GetAcademicClassesByIdsAsync(scheduleDto.AcademicClassIds);

            if (classes.Count == 0)
            {
                _logger.LogWarning("No academic classes found with the provided IDs");
                return false;
            }

            foreach (var academicClass in classes)
            {
                academicClass.RegistrationOpenTime = scheduleDto.RegistrationOpenTime;
                academicClass.RegistrationCloseTime = scheduleDto.RegistrationCloseTime;
                  // Set IsRegistrable based on the current time and the registration times
                var currentTime = DateTime.UtcNow;
                academicClass.IsRegistrable = currentTime >= scheduleDto.RegistrationOpenTime && 
                                             currentTime < scheduleDto.RegistrationCloseTime;
            }

            await _academicClassRepository.SaveChangesAsync();
            
            _logger.LogInformation(
                "Scheduled registration for {Count} academic classes. Open: {OpenTime}, Close: {CloseTime}", 
                classes.Count, 
                scheduleDto.RegistrationOpenTime,
                scheduleDto.RegistrationCloseTime);
            
            return true;
        }

        /// <summary>
        /// Validates that the new academic class schedules don't conflict with existing ones.
        /// A conflict occurs when: in the same semester, if any week overlaps with another class,
        /// the combination of day of week, room, and shift must not all be the same.
        /// </summary>
        private async Task ValidateScheduleConflictsAsync(AcademicClassCreateDto academicClassCreateDto)
        {
            if (academicClassCreateDto.ScheduleInDays == null || academicClassCreateDto.ScheduleInDays.Count == 0)
            {
                return; // No schedules to validate
            }

            // Get all existing academic classes in the same semester with their schedules
            var existingClasses = await _academicClassRepository.GetAcademicClassesBySemesterWithSchedulesAsync(academicClassCreateDto.SemesterId);

            var newClassWeeks = academicClassCreateDto.ListOfWeeks?.ToHashSet() ?? new HashSet<int>();

            foreach (var newSchedule in academicClassCreateDto.ScheduleInDays)
            {
                // Check against all existing classes
                foreach (var existingClass in existingClasses)
                {
                    var existingClassWeeks = existingClass.ListOfWeeks?.ToHashSet() ?? new HashSet<int>();
                    
                    // Check if there's any week overlap
                    bool hasWeekOverlap = newClassWeeks.Intersect(existingClassWeeks).Any();
                    
                    if (hasWeekOverlap)
                    {
                        // Check if any schedule conflicts with the new schedule
                        foreach (var existingSchedule in existingClass.ScheduleInDays)
                        {
                            bool isDayOfWeekSame = string.Equals(newSchedule.DayOfWeek, existingSchedule.DayOfWeek, StringComparison.OrdinalIgnoreCase);
                            bool isRoomSame = newSchedule.RoomId == existingSchedule.RoomId;
                            bool isShiftSame = newSchedule.ShiftId == existingSchedule.ShiftId;

                            if (isDayOfWeekSame && isRoomSame && isShiftSame)
                            {
                                throw new InvalidOperationException(
                                    $"Schedule conflict detected! Another class '{existingClass.Name}' already uses " +
                                    $"Room {existingSchedule.RoomId} on {existingSchedule.DayOfWeek} " +
                                    $"during Shift {existingSchedule.ShiftId} in weeks that overlap with this class. " +
                                    $"Overlapping weeks: {string.Join(", ", newClassWeeks.Intersect(existingClassWeeks))}");
                            }
                        }
                    }
                }
            }
        }

        public async Task<bool> DeleteAcademicClassAsync(Guid id)
        {
            // Check if academic class exists
            var academicClass = await _academicClassRepository.GetAcademicClassByIdAsync(id);
            if (academicClass == null)
            {
                throw new KeyNotFoundException("Academic class not found");
            }

            // Check if academic class has any enrollments using gRPC call
            var enrollmentResponse = await _enrollmentClientService.GetEnrollmentCountAsync(id.ToString());
            if (!enrollmentResponse.Success)
            {
                throw new InvalidOperationException($"Failed to check enrollments: {string.Join(", ", enrollmentResponse.Error)}");
            }

            if (enrollmentResponse.Count > 0)
            {
                throw new InvalidOperationException($"Cannot delete academic class '{academicClass.Name}' because it has {enrollmentResponse.Count} student enrollment(s). Please remove all enrollments before deleting the academic class.");
            }

            // If no enrollments, proceed with deletion
            var result = await _academicClassRepository.DeleteAcademicClassAsync(id);
            return result;
        }
    }
}
