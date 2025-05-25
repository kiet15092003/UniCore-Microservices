using System;
using System.Text.Json.Serialization;
using UserService.Utils.JsonConverters;

namespace UserService.Business.Dtos.Address
{
    public class AddressDto
    {
        [JsonConverter(typeof(StringToNullableGuidConverter))]
        public Guid? Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string AddressDetail { get; set; }
    }
}
