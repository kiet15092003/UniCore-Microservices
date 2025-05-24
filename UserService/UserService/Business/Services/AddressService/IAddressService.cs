using UserService.Business.Dtos.Address;

namespace UserService.Business.Services.AddressService
{
    public interface IAddressService
    {
        Task<AddressDto> GetAddressByIdAsync(Guid id);
        Task<AddressDto> CreateAddressAsync(CreateAddressDto createAddressDto, Guid userId);
        Task<AddressDto> UpdateAddressAsync(Guid id, UpdateAddressDto updateAddressDto);
        Task<bool> DeleteAddressAsync(Guid id);
    }
} 