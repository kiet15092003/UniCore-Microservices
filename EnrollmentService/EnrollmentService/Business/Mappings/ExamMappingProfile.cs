using AutoMapper;
using EnrollmentService.Business.Dtos.Exam;
using EnrollmentService.Entities;

namespace EnrollmentService.Business.Mappings
{    public class ExamMappingProfile : Profile
    {
        public ExamMappingProfile()
        {
            CreateMap<Exam, ExamReadDto>()
                .ForMember(dest => dest.TotalEnrollment, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPassed, opt => opt.Ignore())
                .ForMember(dest => dest.TotalFailed, opt => opt.Ignore())
                .ForMember(dest => dest.AverageScore, opt => opt.Ignore())
                .ForMember(dest => dest.Room, opt => opt.Ignore())
                .ForMember(dest => dest.AcademicClass, opt => opt.Ignore());

            CreateMap<ExamCreateDto, Exam>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.EnrollmentExams, opt => opt.Ignore());

            CreateMap<EnrollmentExam, EnrollmentExamDto>();
        }
    }
}
