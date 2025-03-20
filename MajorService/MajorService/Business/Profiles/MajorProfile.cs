using AutoMapper;
using MajorService.Business.Dtos.Major;
using MajorService.Entities;

namespace MajorService.Business.Profiles
{
    public class MajorProfile : Profile
    {
        public MajorProfile()
        {
            CreateMap<Major, MajorReadDto>();
        }
    }
}
