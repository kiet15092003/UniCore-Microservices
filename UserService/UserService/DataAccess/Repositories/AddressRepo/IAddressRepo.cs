using UserService.Entities;

namespace UserService.DataAccess.Repositories.AddressRepo
{
    public interface IAddressRepo
    {
        Task<Address> GetAddressByIdAsync(Guid id);
        Task<Address> CreateAsync(Address address, Guid userId);
        Task<Address> UpdateAsync(Address address);
        Task<bool> DeleteAsync(Guid id);
    }
} 