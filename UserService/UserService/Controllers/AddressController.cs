using Microsoft.AspNetCore.Mvc;
using UserService.Business.Services.AddressService;
using UserService.Business.Dtos.Address;
using UserService.Middleware;
using System.Security.Claims;

namespace UserService.Controllers
{
    [Route("api/u/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly ILogger<AddressController> _logger;

        public AddressController(IAddressService addressService, ILogger<AddressController> logger)
        {
            _addressService = addressService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<AddressDto>> GetAddress(Guid id)
        {
            try
            {
                var address = await _addressService.GetAddressByIdAsync(id);
                if (address == null)
                {
                    return ApiResponse<AddressDto>.ErrorResponse(["Address not found"]);
                }
                return ApiResponse<AddressDto>.SuccessResponse(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting address with id {Id}", id);
                return ApiResponse<AddressDto>.ErrorResponse([$"Error getting address: {ex.Message}"]);
            }
        }

        [HttpPost]
        public async Task<ApiResponse<AddressDto>> CreateAddress(CreateAddressDto createAddressDto)
        {
            try
            {
                // Lấy userId từ token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return ApiResponse<AddressDto>.ErrorResponse(["User not authenticated"]);
                }

                var address = await _addressService.CreateAddressAsync(createAddressDto, userId);
                return ApiResponse<AddressDto>.SuccessResponse(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating address");
                return ApiResponse<AddressDto>.ErrorResponse([$"Error creating address: {ex.Message}"]);
            }
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<AddressDto>> UpdateAddress(Guid id, UpdateAddressDto updateAddressDto)
        {
            try
            {
                var address = await _addressService.UpdateAddressAsync(id, updateAddressDto);
                if (address == null)
                {
                    return ApiResponse<AddressDto>.ErrorResponse(["Address not found"]);
                }
                return ApiResponse<AddressDto>.SuccessResponse(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address with id {Id}", id);
                return ApiResponse<AddressDto>.ErrorResponse([$"Error updating address: {ex.Message}"]);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteAddress(Guid id)
        {
            try
            {
                var result = await _addressService.DeleteAddressAsync(id);
                if (result)
                {
                    return ApiResponse<bool>.SuccessResponse(true);
                }
                return ApiResponse<bool>.ErrorResponse(["Address not found"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address with id {Id}", id);
                return ApiResponse<bool>.ErrorResponse([$"Error deleting address: {ex.Message}"]);
            }
        }
    }
} 