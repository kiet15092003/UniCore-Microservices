using AutoMapper;
using UserService.Business.Dtos.Auth;

namespace UserService.Business.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserReadDto>();
        }
    }
}
