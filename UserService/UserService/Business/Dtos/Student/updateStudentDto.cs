using System.ComponentModel.DataAnnotations;
using UserService.Business.Dtos.Guardian;
using UserService.Business.Dtos.Address;
namespace UserService.Business.Dtos.Student
{
    public class UpdateStudentDto
    { 
      
        public int? AccumulateCredits { get; set; }
   
        public double? AccumulateScore { get; set; }
     
        public int? AccumulateActivityScore { get; set; }
        public Guid? MajorId { get; set; }
        public Guid? BatchId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PersonId { get; set; }
        public DateTime? Dob { get; set; }
        public string? PhoneNumber { get; set; }
        public int? Status { get; set; }
        public string? ImageUrl { get; set; }
        public List<GuardianDto>? Guardians { get; set; }
        public AddressDto? Address { get; set; }
    }

}
