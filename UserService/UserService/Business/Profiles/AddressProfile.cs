using AutoMapper;
using UserService.Business.Dtos.Address;
using UserService.Entities;

namespace UserService.Business.Profiles
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<Address, AddressDto>();
            CreateMap<AddressDto, Address>();
        }
    }
}
