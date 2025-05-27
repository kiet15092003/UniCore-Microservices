using AutoMapper;
using UserService.Entities;
using UserService.Business.Dtos.Student;
using UserService.Business.Dtos.Address;
using UserService.Business.Dtos.Guardian;
using UserService.Business.Dtos.Batch;

public class StudentProfile : Profile
{
    public StudentProfile()
    {
        CreateMap<ApplicationUser, ApplicationUserDto>();
        CreateMap<ApplicationUser, ApplicationUserDto>()
            .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob.ToString("yyyy-MM-dd")));

        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.ApplicationUser, opt => opt.MapFrom(src => src.ApplicationUser));

        CreateMap<Student, StudentDto>();
        CreateMap<Student, StudentDetailDto>();
        CreateMap<Student, StudentDetailDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ApplicationUser.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ApplicationUser.Email))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.ApplicationUser.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.ApplicationUser.LastName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.ApplicationUser.PhoneNumber))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.ApplicationUser.Status))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ApplicationUser.ImageUrl))
            .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.ApplicationUser.PersonId))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ApplicationUser.Address))
            .ForMember(dest => dest.Guardians, opt => opt.MapFrom(src => src.Guardians))
            .ForMember(dest => dest.BatchName, opt => opt.MapFrom(src => src.Batch.Title))
            .ForMember(dest => dest.BatchYear, opt => opt.MapFrom(src => src.Batch.StartYear));
    }
}