using Grpc.Core;
using MajorService;
using MajorService.DataAccess;
using MajorService.Entities;
using Microsoft.EntityFrameworkCore;

namespace MajorService.CommunicationTypes.Grpc
{
    public class GrpcDepartmentService : GrpcDepartment.GrpcDepartmentBase
    {
        private readonly AppDbContext _context;

        public GrpcDepartmentService(AppDbContext context)
        {
            _context = context;
        }

        public override async Task<DepartmentResponse> GetDepartmentById(DepartmentRequest request, ServerCallContext context)
        {
            var errors = new List<string>();

            // Validate GUID format
            if (!Guid.TryParse(request.Id, out Guid departmentId))
            {
                errors.Add("Invalid Department ID format.");
            }

            // Query database if ID is valid
            Department? department = null;
            if (errors.Count == 0)
            {
                department = await _context.Departments
                    .Include(d => d.MajorGroups)
                    .FirstOrDefaultAsync(d => d.Id == departmentId);
                
                if (department == null)
                {
                    errors.Add("Department not found.");
                }
            }

            // Return error response if any errors exist
            if (errors.Count > 0)
            {
                return new DepartmentResponse
                {
                    Success = false,
                    Error = { errors }
                };
            }

            // Map major groups
            var majorGroupsData = new List<MajorGroupData>();
            if (department!.MajorGroups != null)
            {
                foreach (var majorGroup in department.MajorGroups)
                {
                    majorGroupsData.Add(new MajorGroupData
                    {
                        Id = majorGroup.Id.ToString(),
                        Name = majorGroup.Name
                    });
                }
            }

            // Return success response
            return new DepartmentResponse
            {
                Success = true,
                Data = new DepartmentData
                {
                    Id = department.Id.ToString(),
                    Name = department.Name,
                    Code = department.Code,
                    MajorGroups = { majorGroupsData }
                }
            };
        }
    }
} 