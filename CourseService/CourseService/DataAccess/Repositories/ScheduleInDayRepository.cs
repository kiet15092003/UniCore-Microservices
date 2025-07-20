using CourseService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseService.DataAccess.Repositories
{
    public class ScheduleInDayRepository : IScheduleInDayRepository
    {
        private readonly AppDbContext _context;

        public ScheduleInDayRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ScheduleInDay> CreateScheduleInDayAsync(ScheduleInDay scheduleInDay)
        {
            var newScheduleInDay = await _context.ScheduleInDays.AddAsync(scheduleInDay);
            await _context.SaveChangesAsync();
            return newScheduleInDay.Entity;
        }

        public async Task<ScheduleInDay?> GetScheduleInDayByIdAsync(Guid id)
        {
            return await _context.ScheduleInDays
                .Include(s => s.Shift)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<ScheduleInDay?> GetScheduleInDayByAcademicClassIdAsync(Guid academicClassId)
        {
            return await _context.ScheduleInDays
                .Include(s => s.Shift)
                .FirstOrDefaultAsync(s => s.AcademicClassId == academicClassId);
        }

        public async Task<List<ScheduleInDay>> GetScheduleInDaysByShiftIdAsync(Guid shiftId)
        {
            return await _context.ScheduleInDays
                .Where(s => s.ShiftId == shiftId)
                .Include(s => s.AcademicClass)
                .ToListAsync();
        }

        public async Task<ScheduleInDay> UpdateScheduleInDayAsync(ScheduleInDay scheduleInDay)
        {
            _context.ScheduleInDays.Update(scheduleInDay);
            await _context.SaveChangesAsync();
            return scheduleInDay;
        }

        public async Task<List<ScheduleInDay>> GetScheduleInDaysByAcademicClassIdAsync(Guid academicClassId)
        {
            return await _context.ScheduleInDays
                .Include(s => s.Shift)
                .Where(s => s.AcademicClassId == academicClassId)
                .ToListAsync();
        }

        public async Task<bool> DeleteScheduleInDayAsync(Guid id)
        {
            var scheduleInDay = await _context.ScheduleInDays.FindAsync(id);
            if (scheduleInDay == null)
            {
                return false;
            }

            _context.ScheduleInDays.Remove(scheduleInDay);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
