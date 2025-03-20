using Grpc.Core;
using MajorService;
using MajorService.DataAccess;
using MajorService.Entities;
using Microsoft.EntityFrameworkCore;

public class GrpcMajorService : GrpcMajor.GrpcMajorBase
{
    private readonly AppDbContext _context;

    public GrpcMajorService(AppDbContext context)
    {
        _context = context;
    }

    public override async Task<MajorResponse> GetMajorById(MajorRequest request, ServerCallContext context)
    {
        var errors = new List<string>();

        // Validate GUID format
        if (!Guid.TryParse(request.Id, out Guid majorId))
        {
            errors.Add("Invalid Major ID format.");
        }

        // Query database if ID is valid
        Major? major = null;
        if (errors.Count == 0)
        {
            major = await _context.Majors.FirstOrDefaultAsync(m => m.Id == majorId);
            if (major == null)
            {
                errors.Add("Major not found.");
            }
        }

        // Return error response if any errors exist
        if (errors.Count > 0)
        {
            return new MajorResponse
            {
                Success = false,
                Error = { errors }
            };
        }

        // Return success response
        return new MajorResponse
        {
            Success = true,
            Data = new MajorData
            {
                Id = major!.Id.ToString(),
                Name = major.Name,
                Code = major.Code
            }
        };
    }
}
