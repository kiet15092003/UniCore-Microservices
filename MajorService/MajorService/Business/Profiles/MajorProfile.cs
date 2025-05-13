using AutoMapper;
using MajorService.Business.Dtos.Major;
using MajorService.Entities;

namespace MajorService.Business.Profiles
{
    public class MajorProfile : Profile
    {
        public MajorProfile()
        {
            CreateMap<Major, MajorReadDto>()
                .ForMember(dest => dest.MajorGroup, opt => opt.MapFrom(src => src.MajorGroup))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
