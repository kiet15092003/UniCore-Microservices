using AutoMapper;
using EnrollmentService.Business.Dtos.StudentResult;
using EnrollmentService.Entities;

namespace EnrollmentService.Business.Mappings
{
    public class StudentResultMappingProfile : Profile
    {
        public StudentResultMappingProfile()
        {
            CreateMap<StudentResult, StudentResultDto>()
                .ForMember(dest => dest.ScoreTypeName, opt => opt.MapFrom(src => src.ScoreType.Type))
                .ForMember(dest => dest.ScoreTypePercentage, opt => opt.MapFrom(src => src.ScoreType.Percentage));

            CreateMap<UpdateStudentResultDto, StudentResult>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
} 