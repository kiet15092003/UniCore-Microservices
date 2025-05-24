using UserService.Business.Dtos.Guardian;
using UserService.Business.Dtos.Address;
using System;

namespace UserService.Business.Dtos.Student
{
    public class GetStudentByIdDto
    {
        public Guid Id { get; set; }
        public string StudentCode { get; set; }
        public Guid MajorId { get; set; }
        public Guid BatchId { get; set; }
        public ApplicationUserDto ApplicationUser { get; set; }
        public List<GuardianDto>? Guardians { get; set; }
        public AddressDto? Address { get; set; }
    }
}
