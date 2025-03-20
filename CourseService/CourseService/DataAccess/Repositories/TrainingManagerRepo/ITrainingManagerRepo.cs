using CourseService.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CourseService.DataAccess.Repositories.TrainingManagerRepo
{
    public interface ITrainingManagerRepo
    {
        Task<IEnumerable<TrainingManager>> GetAllTrainingManagerAsync();
        Task<TrainingManager> CreateTrainingManagerAsync(TrainingManager trainingManager);
    }
}
