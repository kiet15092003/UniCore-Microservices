using AutoMapper;
using CourseService.Business.Dtos.AcademicClass;
using CourseService.CommunicationTypes.Grpc.GrpcClient;
using CourseService.DataAccess.Repositories;
using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;
using MajorService;

namespace CourseService.Business.Services
{
    public class AcademicClassService : IAcademicClassService
    {        
        private readonly IAcademicClassRepository _academicClassRepository;
        private readonly IScheduleInDayRepository _scheduleInDayRepository;
        private readonly IMapper _mapper;
        private readonly GrpcRoomClientService _roomClientService;
        private readonly ILogger<AcademicClassService> _logger;

        public AcademicClassService(
            IAcademicClassRepository academicClassRepository,
            IScheduleInDayRepository scheduleInDayRepository,
            GrpcRoomClientService roomClientService,
            IMapper mapper,
            ILogger<AcademicClassService> logger)
        {            _academicClassRepository = academicClassRepository;
            _scheduleInDayRepository = scheduleInDayRepository;
            _roomClientService = roomClientService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<AcademicClassReadDto> CreateAcademicClassAsync(AcademicClassCreateDto academicClassCreateDto)
        {
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

            // Populate room data for each schedule
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
        public async Task<List<AcademicClassReadDto>> GetAcademicClassesByCourseIdAsync(Guid courseId)
        {
            var academicClasses = await _academicClassRepository.GetAcademicClassesByCourseIdAsync(courseId);
            var academicClassDtos = _mapper.Map<List<AcademicClassReadDto>>(academicClasses);

            // Populate room data for each academic class
            await PopulateRoomDataForClasses(academicClassDtos);

            return academicClassDtos;
        }

        public async Task<List<AcademicClassReadDto>> GetAcademicClassesBySemesterIdAsync(Guid semesterId)
        {
            var academicClasses = await _academicClassRepository.GetAcademicClassesBySemesterIdAsync(semesterId);
            var academicClassDtos = _mapper.Map<List<AcademicClassReadDto>>(academicClasses);
            
            // Populate room data for each academic class
            await PopulateRoomDataForClasses(academicClassDtos);
            
            return academicClassDtos;
        }
        
        public async Task<List<AcademicClassReadDto>> GetAcademicClassesForMajorAsync(Guid majorId)
        {
            var academicClasses = await _academicClassRepository.GetAcademicClassesForMajorAsync(majorId);
            var academicClassDtos = _mapper.Map<List<AcademicClassReadDto>>(academicClasses);
            
            // Populate room data for each academic class
            await PopulateRoomDataForClasses(academicClassDtos);
            
            return academicClassDtos;
        }
        
        // Helper method to populate room data for a list of academic classes
        private async Task PopulateRoomDataForClasses(List<AcademicClassReadDto> academicClasses)
        {
            foreach (var academicClass in academicClasses)
            {
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
            }
        }
        public async Task<AcademicClassListResponse> GetAllAcademicClassesPaginationAsync(
            Pagination pagination,
            AcademicClassFilterParams? filterParams,
            Order? order)
        {
            var paginationResult = await _academicClassRepository.GetAllAcademicClassesPaginationAsync(pagination, filterParams, order);

            var academicClassDtos = _mapper.Map<List<AcademicClassReadDto>>(paginationResult.Data);

            // Populate room data for each academic class
            await PopulateRoomDataForClasses(academicClassDtos);

            var response = new AcademicClassListResponse
            {
                Data = academicClassDtos,                
                Total = paginationResult.Total,
                PageSize = paginationResult.PageSize,
                PageIndex = paginationResult.PageIndex
            };

            return response;
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
    }
}
