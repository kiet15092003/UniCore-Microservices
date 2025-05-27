using AutoMapper;
using UserService.Business.Dtos.Guardian;
using UserService.Entities;

namespace UserService.Business.Profiles
{
    public class GuardianProfile : Profile
    {
        public GuardianProfile()
        {
            CreateMap<Guardian, GuardianDto>();
            CreateMap<CreateGuardianDto, Guardian>();
            CreateMap<UpdateGuardianDto, Guardian>();
            CreateMap<GuardianDto, Guardian>();
            CreateMap<GuardianDto, UpdateGuardianDto>();
        }
    }
}