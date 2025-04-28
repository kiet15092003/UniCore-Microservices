using AutoMapper;
using CourseService.Business.Dtos.TrainingRoadmap;
using CourseService.Entities;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Profiles
{
    public class TrainingRoadmapProfile : Profile
    {
        public TrainingRoadmapProfile()
        {
            // TrainingRoadmap -> TrainingRoadmapReadDto
            CreateMap<TrainingRoadmap, TrainingRoadmapReadDto>();

            // TrainingRoadmapCreateDto -> TrainingRoadmap
            CreateMap<TrainingRoadmapCreateDto, TrainingRoadmap>();
            
            // TrainingRoadmapUpdateDto -> TrainingRoadmap
            CreateMap<TrainingRoadmapUpdateDto, TrainingRoadmap>();
            
            // PaginationResult<TrainingRoadmap> -> TrainingRoadmapListResponse
            CreateMap<PaginationResult<TrainingRoadmap>, TrainingRoadmapListResponse>()
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
                .ForMember(dest => dest.PageIndex, opt => opt.MapFrom(src => src.PageIndex));
        }
    }
}