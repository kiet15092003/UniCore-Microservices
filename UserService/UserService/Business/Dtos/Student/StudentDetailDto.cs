using UserService.Business.Dtos.Address;
using UserService.Business.Dtos.Guardian;

namespace UserService.Business.Dtos.Student
{
    public class StudentDetailDto
    {
        public Guid Id { get; set; }
        public string StudentCode { get; set; }
        public double AccumulateCredits { get; set; }
        public double AccumulateScore { get; set; }
        public double AccumulateActivityScore { get; set; }
        public Guid MajorId { get; set; }
        public string MajorName { get; set; }
        public Guid BatchId { get; set; }
        public string BatchName { get; set; }
        public int BatchYear { get; set; }

        // User information
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public string ImageUrl { get; set; }

        // Address information
        public AddressDto Address { get; set; }

        // Guardian information
        public List<GuardianDto> Guardians { get; set; }
    }
} 