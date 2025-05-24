using AutoMapper;
using CourseService.Business.Dtos.AcademicClass;
using CourseService.DataAccess.Repositories;
using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Services
{
    public class AcademicClassService : IAcademicClassService
    {
        private readonly IAcademicClassRepository _academicClassRepository;
        private readonly IScheduleInDayRepository _scheduleInDayRepository;
        private readonly IMapper _mapper;

        public AcademicClassService(
            IAcademicClassRepository academicClassRepository,
            IScheduleInDayRepository scheduleInDayRepository,
            IMapper mapper)
        {
            _academicClassRepository = academicClassRepository;
            _scheduleInDayRepository = scheduleInDayRepository;
            _mapper = mapper;
        }

        public async Task<AcademicClassReadDto> CreateAcademicClassAsync(AcademicClassCreateDto academicClassCreateDto)
        {
            // Create AcademicClass entity
            var academicClass = _mapper.Map<AcademicClass>(academicClassCreateDto);
            
            // Create and save the AcademicClass first to get its Id
            var createdAcademicClass = await _academicClassRepository.CreateAcademicClassAsync(academicClass);

            // Create ScheduleInDay
            var scheduleInDay = new ScheduleInDay
            {
                DayOfWeek = academicClassCreateDto.DayOfWeek,
                RoomId = academicClassCreateDto.RoomId,
                ShiftId = academicClassCreateDto.ShiftId,
                AcademicClassId = createdAcademicClass.Id
            };

            // Save the ScheduleInDay
            var createdSchedule = await _scheduleInDayRepository.CreateScheduleInDayAsync(scheduleInDay);

            // Update the AcademicClass with the ScheduleInDay ID
            createdAcademicClass.ScheduleInDayId = createdSchedule.Id;
            await _academicClassRepository.UpdateAcademicClassAsync(createdAcademicClass);

            // Get the complete AcademicClass with its relationships
            var result = await _academicClassRepository.GetAcademicClassByIdAsync(createdAcademicClass.Id);
            
            return _mapper.Map<AcademicClassReadDto>(result);
        }

        public async Task<AcademicClassReadDto> GetAcademicClassByIdAsync(Guid id)
        {
            var academicClass = await _academicClassRepository.GetAcademicClassByIdAsync(id);
            
            if (academicClass == null)
            {
                throw new Exception($"Academic class with ID {id} not found");
            }
            
            return _mapper.Map<AcademicClassReadDto>(academicClass);
        }

        public async Task<List<AcademicClassReadDto>> GetAcademicClassesByCourseIdAsync(Guid courseId)
        {
            var academicClasses = await _academicClassRepository.GetAcademicClassesByCourseIdAsync(courseId);
            return _mapper.Map<List<AcademicClassReadDto>>(academicClasses);
        }

        public async Task<List<AcademicClassReadDto>> GetAcademicClassesBySemesterIdAsync(Guid semesterId)
        {
            var academicClasses = await _academicClassRepository.GetAcademicClassesBySemesterIdAsync(semesterId);
            return _mapper.Map<List<AcademicClassReadDto>>(academicClasses);
        }
        public async Task<AcademicClassListResponse> GetAllAcademicClassesPaginationAsync(
            Pagination pagination,
            AcademicClassFilterParams? filterParams,
            Order? order)
        {
            var paginationResult = await _academicClassRepository.GetAllAcademicClassesPaginationAsync(pagination, filterParams, order);

            var response = new AcademicClassListResponse
            {
                Data = _mapper.Map<List<AcademicClassReadDto>>(paginationResult.Data),
                Total = paginationResult.Total,
                PageSize = paginationResult.PageSize,
                PageIndex = paginationResult.PageIndex
            };

            return response;
        }
    }
}
