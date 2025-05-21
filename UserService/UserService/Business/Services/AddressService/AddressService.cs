using AutoMapper;
using UserService.DataAccess.Repositories.AddressRepo;
using UserService.Entities;
using UserService.Business.Dtos.Address;

namespace UserService.Business.Services.AddressService
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepo _addressRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<AddressService> _logger;

        public AddressService(
            IAddressRepo addressRepo,
            IMapper mapper,
            ILogger<AddressService> logger)
        {
            _addressRepo = addressRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AddressDto> GetAddressByIdAsync(Guid id)
        {
            try
            {
                var address = await _addressRepo.GetAddressByIdAsync(id);
                return _mapper.Map<AddressDto>(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting address with id {Id}", id);
                throw;
            }
        }

        public async Task<AddressDto> CreateAddressAsync(CreateAddressDto createAddressDto, Guid userId)
        {
            try
            {
                var address = _mapper.Map<Address>(createAddressDto);
                var createdAddress = await _addressRepo.CreateAsync(address, userId);
                return _mapper.Map<AddressDto>(createdAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating address for user {UserId}", userId);
                throw;
            }
        }

        public async Task<AddressDto> UpdateAddressAsync(Guid id, UpdateAddressDto updateAddressDto)
        {
            try
            {
                var existingAddress = await _addressRepo.GetAddressByIdAsync(id);
                if (existingAddress == null)
                {
                    return null;
                }

                _mapper.Map(updateAddressDto, existingAddress);
                var updatedAddress = await _addressRepo.UpdateAsync(existingAddress);
                return _mapper.Map<AddressDto>(updatedAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address with id {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAddressAsync(Guid id)
        {
            try
            {
                return await _addressRepo.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address with id {Id}", id);
                throw;
            }
        }
    }
} 