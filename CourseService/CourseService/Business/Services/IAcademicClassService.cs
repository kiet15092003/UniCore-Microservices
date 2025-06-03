using CourseService.Business.Dtos.AcademicClass;
using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Services
{
    public interface IAcademicClassService
    {
        Task<AcademicClassReadDto> CreateAcademicClassAsync(AcademicClassCreateDto academicClassCreateDto);
        Task<AcademicClassReadDto> GetAcademicClassByIdAsync(Guid id);
        Task<List<AcademicClassReadDto>> GetAcademicClassesByCourseIdAsync(Guid courseId);
        Task<List<AcademicClassReadDto>> GetAcademicClassesBySemesterIdAsync(Guid semesterId);
        Task<List<AcademicClassReadDto>> GetAcademicClassesForMajorAsync(Guid majorId);
        Task<AcademicClassListResponse> GetAllAcademicClassesPaginationAsync(
            Pagination pagination, 
            AcademicClassFilterParams? filterParams,
            Order? order);
        Task<bool> ScheduleRegistrationAsync(ClassRegistrationScheduleDto scheduleDto);
    }
}
