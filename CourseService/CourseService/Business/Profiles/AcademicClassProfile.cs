using AutoMapper;
using CourseService.Business.Dtos.AcademicClass;
using CourseService.CommunicationTypes.Grpc.GrpcClient;
using CourseService.Entities;
using CourseService.Utils.Pagination;
using System.Threading.Tasks;

namespace CourseService.Business.Profiles
{
    public class AcademicClassProfile : Profile
    {
        public AcademicClassProfile()
        {
            // Map schedule
            CreateMap<ScheduleInDay, ScheduleInDayReadDto>()
                .ForMember(dest => dest.ShiftId, opt => opt.MapFrom(src => src.ShiftId))
                .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek))
                .ForMember(dest => dest.Room, opt => opt.Ignore()); // Room will be populated by the service
                  // Map academic class
            CreateMap<AcademicClass, AcademicClassReadDto>()
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course))
                .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester))
                .ForMember(dest => dest.ScheduleInDays, opt => opt.MapFrom(src => src.ScheduleInDays));
                  // Map create dto to entity
            CreateMap<AcademicClassCreateDto, AcademicClass>();
            CreateMap<ScheduleInDayCreateDto, ScheduleInDay>();
            CreateMap<ScheduleInDayCreateForClassDto, ScheduleInDay>();
            
            CreateMap<PaginationResult<AcademicClass>, AcademicClassListResponse>();
        }
    }
}
