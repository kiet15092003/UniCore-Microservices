using System.ComponentModel.DataAnnotations;

namespace UserService.Business.Dtos.Address
{
    public class CreateAddressDto
    {
        [Required]
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string District { get; set; }
        [Required]
        public string Ward { get; set; }
        [Required]
        public string AddressDetail { get; set; }
        public Guid UserId { get; set; }
    }
} 