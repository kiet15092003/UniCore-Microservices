using System;

namespace UserService.Business.Dtos.Address
{
    public class AddressDto
    {
        public Guid Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string AddressDetail { get; set; }
    }
}
