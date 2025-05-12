using AutoMapper;
using CourseService.Business.Dtos.CoursesGroup;
using CourseService.Entities;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Profiles
{
    public class CoursesGroupProfile : Profile
    {
        public CoursesGroupProfile()
        {
            // CoursesGroup -> CoursesGroupReadDto
            CreateMap<CoursesGroup, CoursesGroupReadDto>()
                .ForMember(dest => dest.CourseIds, opt => opt.MapFrom(src => src.CourseIds ?? new List<Guid>()))
                .ForMember(dest => dest.Courses, opt => opt.Ignore()); // Courses will be populated separately in the service
            
            // CoursesGroupCreateDto -> CoursesGroup
            CreateMap<CoursesGroupCreateDto, CoursesGroup>()
                .ForMember(dest => dest.CourseIds, opt => opt.Ignore()); // We handle this manually in the service
            
            // PaginationResult<CoursesGroup> -> CoursesGroupListResponse
            CreateMap<PaginationResult<CoursesGroup>, CoursesGroupListResponse>()
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
                .ForMember(dest => dest.PageIndex, opt => opt.MapFrom(src => src.PageIndex))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize));
        }
    }
}