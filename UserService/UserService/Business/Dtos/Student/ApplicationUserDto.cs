using System;

namespace UserService.Business.Dtos.Student
{
    public class ApplicationUserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonId { get; set; }
        public string Dob { get; set; }
        public string PhoneNumber { get; set; }
        public int Status { get; set; }
        public string ImageUrl { get; set; }
    }
} 