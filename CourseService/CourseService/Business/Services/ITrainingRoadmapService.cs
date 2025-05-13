using CourseService.Business.Dtos.TrainingRoadmap;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Services
{    public interface ITrainingRoadmapService
    {
        Task<TrainingRoadmapReadDto> CreateTrainingRoadmapAsync(TrainingRoadmapCreateDto createDto);
        Task<TrainingRoadmapListResponse> GetTrainingRoadmapsByPagination(Pagination pagination, TrainingRoadmapFilterParams filterParams, Order? order);
        Task<TrainingRoadmapReadDto> UpdateTrainingRoadmapAsync(Guid id, TrainingRoadmapUpdateDto updateDto);
        Task<TrainingRoadmapReadDto> GetTrainingRoadmapByIdAsync(Guid id);
        Task<TrainingRoadmapReadDto> AddTrainingRoadmapComponentsAsync(TrainingRoadmapAddComponentsDto componentsDto);
        Task<TrainingRoadmapReadDto> DeactivateTrainingRoadmapAsync(Guid id);
        Task<TrainingRoadmapReadDto> ActivateTrainingRoadmapAsync(Guid id);
    }
}