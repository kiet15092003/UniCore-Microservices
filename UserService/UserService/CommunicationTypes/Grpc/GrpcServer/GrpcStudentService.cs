using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using UserService.DataAccess;
using UserService.Entities;

namespace UserService.CommunicationTypes.Grpc.GrpcServer
{
    public class GrpcStudentService : GrpcStudent.GrpcStudentBase
    {
        private readonly AppDbContext _context;

        public GrpcStudentService(AppDbContext context)
        {
            _context = context;
        }

        public override async Task<StudentResponse> GetStudentById(StudentRequest request, ServerCallContext context)
        {
            var errors = new List<string>();

            // Validate GUID format
            if (!Guid.TryParse(request.Id, out Guid studentId))
            {
                errors.Add("Invalid Student ID format.");
            }

            // Query database if ID is valid
            Student? student = null;
            if (errors.Count == 0)
            {
                student = await _context.Students
                    .Include(s => s.ApplicationUser)
                    .FirstOrDefaultAsync(s => s.Id == studentId);
                
                if (student == null)
                {
                    errors.Add("Student not found.");
                }
            }

            // Return error response if any errors exist
            if (errors.Count > 0)
            {
                return new StudentResponse
                {
                    Success = false,
                    Error = { errors }
                };
            }

            // Return success response
            var response = new StudentResponse
            {
                Success = true,
                Data = new StudentData
                {
                    Id = student!.Id.ToString(),
                    StudentCode = student.StudentCode,
                    AccumulateCredits = student.AccumulateCredits,
                    AccumulateScore = student.AccumulateScore,
                    AccumulateActivityScore = student.AccumulateActivityScore,
                    MajorId = student.MajorId.ToString(),
                    BatchId = student.BatchId.ToString(),
                    ApplicationUserId = student.ApplicationUserId,
                    User = new UserData
                    {
                        Id = student.ApplicationUser.Id,
                        FirstName = student.ApplicationUser.FirstName,
                        LastName = student.ApplicationUser.LastName,
                        Email = student.ApplicationUser.Email,
                        PhoneNumber = student.ApplicationUser.PhoneNumber,
                        PersonId = student.ApplicationUser.PersonId,
                        ImageUrl = student.ApplicationUser.ImageUrl ?? ""
                    }
                }
            };

            return response;
        }
    }
}
