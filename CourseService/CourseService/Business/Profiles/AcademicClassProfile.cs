using AutoMapper;
using CourseService.Business.Dtos.AcademicClass;
using CourseService.CommunicationTypes.Grpc.GrpcClient;
using CourseService.Entities;
using CourseService.Utils.Pagination;
using System.Threading.Tasks;

namespace CourseService.Business.Profiles
{
    public class AcademicClassProfile : Profile
    {        public AcademicClassProfile()
        {
            // Map schedule
            CreateMap<ScheduleInDay, ScheduleInDayReadDto>()
                .ForMember(dest => dest.ShiftId, opt => opt.MapFrom(src => src.ShiftId))
                .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek))
                .ForMember(dest => dest.Room, opt => opt.Ignore()); // Room will be populated by the service
            
            // Map basic academic class (for avoiding circular references)
            CreateMap<AcademicClass, AcademicClassBasicDto>()
                .ForMember(dest => dest.ScheduleInDays, opt => opt.MapFrom(src => src.ScheduleInDays));  
            
            // Map academic class
            CreateMap<AcademicClass, AcademicClassReadDto>()
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course))
                .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester))
                .ForMember(dest => dest.ScheduleInDays, opt => opt.MapFrom(src => src.ScheduleInDays))
                .ForMember(dest => dest.ParentTheoryAcademicClassId, opt => opt.MapFrom(src => src.ParentTheoryAcademicClassId))
                .ForMember(dest => dest.ParentTheoryAcademicClass, opt => opt.MapFrom(src => src.ParentTheoryAcademicClass))
                .ForMember(dest => dest.ChildPracticeAcademicClassIds, opt => opt.MapFrom(src => src.ChildPracticeAcademicClassIds))
                .ForMember(dest => dest.ChildPracticeAcademicClasses, opt => opt.MapFrom(src => src.ChildPracticeAcademicClasses));
            
            CreateMap<AcademicClassCreateDto, AcademicClass>();
            CreateMap<ScheduleInDayCreateForClassDto, ScheduleInDay>();
            
            CreateMap<PaginationResult<AcademicClass>, AcademicClassListResponse>();
        }
    }
}
