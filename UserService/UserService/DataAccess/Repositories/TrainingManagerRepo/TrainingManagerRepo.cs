using UserService.Entities;

namespace UserService.DataAccess.Repositories.TrainingManagerRepo
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
            await _context.TrainingManagers.AddAsync(trainingManager);
            await _context.SaveChangesAsync();
            return trainingManager;
        }
    }
}
