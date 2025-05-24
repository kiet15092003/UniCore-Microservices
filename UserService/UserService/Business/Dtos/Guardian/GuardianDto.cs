using System;

namespace UserService.Business.Dtos.Guardian
{
    public class GuardianDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Relationship { get; set; }
    }
}
