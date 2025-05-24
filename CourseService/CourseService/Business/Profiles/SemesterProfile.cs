using AutoMapper;
using CourseService.Business.Dtos.Semester;
using CourseService.Entities;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Profiles
{
    public class SemesterProfile : Profile
    {
        public SemesterProfile()
        {
            CreateMap<SemesterCreateDto, Semester>();
            CreateMap<SemesterUpdateDto, Semester>();
            CreateMap<Semester, SemesterReadDto>();
            
            // For pagination results
            CreateMap<PaginationResult<Semester>, PaginationResult<SemesterReadDto>>()
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data));
        }
    }
}
