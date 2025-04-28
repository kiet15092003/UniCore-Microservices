using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.DataAccess.Repositories
{
    public interface ITrainingRoadmapRepository
    {
        Task<TrainingRoadmap> CreateTrainingRoadmapAsync(TrainingRoadmap trainingRoadmap);
        Task<PaginationResult<TrainingRoadmap>> GetAllTrainingRoadmapsPaginationAsync(
                Pagination pagination,
                TrainingRoadmapFilterParams filterParams,
                Order? order);
        Task<TrainingRoadmap> GetTrainingRoadmapByIdAsync(Guid id);
        Task<TrainingRoadmap> UpdateTrainingRoadmapAsync(TrainingRoadmap trainingRoadmap);
    }
}