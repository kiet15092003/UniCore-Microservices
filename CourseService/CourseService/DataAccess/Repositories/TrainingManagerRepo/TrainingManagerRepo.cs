using CourseService.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseService.DataAccess.Repositories.TrainingManagerRepo
{
    public class TrainingManagerRepo : ITrainingManagerRepo
    {
        private readonly AppDbContext _context;
        public TrainingManagerRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TrainingManager> CreateTrainingManagerAsync(TrainingManager trainingManager)
        {
            var newTrainingManager = await _context.TrainingManagers.AddAsync(trainingManager);
            await _context.SaveChangesAsync();
            return newTrainingManager.Entity;
        }

        public async Task<IEnumerable<TrainingManager>> GetAllTrainingManagerAsync()
        {
            return await _context.TrainingManagers.ToListAsync();
        }
    }
}
