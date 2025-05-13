using AutoMapper;
using UserService.Entities;
using UserService.Business.Dtos.Student;

public class StudentProfile : Profile
{
    public StudentProfile()
    {
         CreateMap<ApplicationUser, ApplicationUserDto>()
            .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob.ToString("yyyy-MM-dd")));

        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.ApplicationUser, opt => opt.MapFrom(src => src.ApplicationUser));

    }
}