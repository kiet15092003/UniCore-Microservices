using CourseService.Entities;

namespace CourseService.DataAccess.Repositories
{
    public interface IScheduleInDayRepository
    {
        Task<ScheduleInDay> CreateScheduleInDayAsync(ScheduleInDay scheduleInDay);
        Task<ScheduleInDay?> GetScheduleInDayByIdAsync(Guid id);
        Task<ScheduleInDay?> GetScheduleInDayByAcademicClassIdAsync(Guid academicClassId);
        Task<List<ScheduleInDay>> GetScheduleInDaysByAcademicClassIdAsync(Guid academicClassId);
        Task<List<ScheduleInDay>> GetScheduleInDaysByShiftIdAsync(Guid shiftId);
        Task<ScheduleInDay> UpdateScheduleInDayAsync(ScheduleInDay scheduleInDay);
        Task<bool> DeleteScheduleInDayAsync(Guid id);
    }
}
