using Microsoft.EntityFrameworkCore;
using UserService.Entities;
using UserService.Utils.Pagination;
using UserService.Utils.Filter;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace UserService.DataAccess.Repositories.AddressRepo
{
    public class AddressRepo : IAddressRepo
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AddressRepo> _logger;

        public AddressRepo(AppDbContext context, ILogger<AddressRepo> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Address> GetAddressByIdAsync(Guid id)
        {
            var result = await _context.Addresses
                .FirstOrDefaultAsync(d => d.Id == id);
            if (result == null)
            {
                throw new KeyNotFoundException("Address not found");
            }
            return result;
        }

        public async Task<Address> CreateAsync(Address address, Guid userId)
        {
            // Kiểm tra user có tồn tại không
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Kiểm tra user đã có address chưa
            if (user.AddressId.HasValue)
            {
                throw new InvalidOperationException("User already has an address");
            }

            // Tạo address mới
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();

            // Cập nhật AddressId cho user
            user.AddressId = address.Id;
            await _context.SaveChangesAsync();

            return address;
        }

        public async Task<Address> UpdateAsync(Address address)
        {
            _logger.LogInformation("-----------------------------------56 {address}", JsonSerializer.Serialize(address));
            var existingAddress = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == address.Id);

            if (existingAddress == null)
                throw new KeyNotFoundException("Address not found");

            existingAddress.Country = address.Country;
            existingAddress.City = address.City;
            existingAddress.District = address.District;
            existingAddress.Ward = address.Ward;
            existingAddress.AddressDetail = address.AddressDetail;

            _context.Addresses.Update(existingAddress);
            await _context.SaveChangesAsync();

            return existingAddress;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id);
            
            if (address == null)
                return false;

            // Tìm user đang sử dụng address này
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.AddressId == id);
            
            if (user != null)
            {
                user.AddressId = null;
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 