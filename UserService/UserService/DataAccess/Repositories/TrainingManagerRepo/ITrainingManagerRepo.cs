using UserService.Entities;

namespace UserService.DataAccess.Repositories.TrainingManagerRepo
{
    public interface ITrainingManagerRepo
    {
        Task<TrainingManager> CreateTrainingManagerAsync(TrainingManager trainingManager);
    }
}
