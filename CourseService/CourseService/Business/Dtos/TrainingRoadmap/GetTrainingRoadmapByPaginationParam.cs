using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Dtos.TrainingRoadmap
{
    public class GetTrainingRoadmapByPaginationParam
    {
        public Pagination Pagination { get; set; } = new Pagination();
        public TrainingRoadmapFilterParams Filter { get; set; } = new TrainingRoadmapFilterParams();
        public Order Order { get; set; } = new Order();
    }
}