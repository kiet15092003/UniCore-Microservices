using AutoMapper;
using CourseService.Business.Dtos.AcademicClass;
using CourseService.Entities;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Profiles
{
    public class AcademicClassProfile : Profile
    {
        public AcademicClassProfile()
        {
            // Map schedule
            CreateMap<ScheduleInDay, ScheduleInDayReadDto>()
                .ForMember(dest => dest.ShiftId, opt => opt.MapFrom(src => src.ShiftId))
                .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek));
                
            // Map academic class
            CreateMap<AcademicClass, AcademicClassReadDto>()
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course))
                .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester))
                .ForMember(dest => dest.ScheduleInDay, opt => opt.MapFrom(src => src.ScheduleInDay));
                
            // Map create dto to entity
            CreateMap<AcademicClassCreateDto, AcademicClass>();
            CreateMap<ScheduleInDayCreateDto, ScheduleInDay>();
            
            CreateMap<PaginationResult<AcademicClass>, AcademicClassListResponse>();
        }
    }
}
