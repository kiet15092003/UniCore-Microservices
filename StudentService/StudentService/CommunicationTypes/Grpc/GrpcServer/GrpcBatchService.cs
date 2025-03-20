using Grpc.Core;
using StudentService;
using StudentService.DataAccess;
using StudentService.Entities;
using Microsoft.EntityFrameworkCore;

public class GrpcBatchService : GrpcBatch.GrpcBatchBase
{
    private readonly AppDbContext _context;

    public GrpcBatchService(AppDbContext context)
    {
        _context = context;
    }

    public override async Task<BatchResponse> GetBatchById(BatchRequest request, ServerCallContext context)
    {
        var errors = new List<string>();

        // Validate GUID format
        if (!Guid.TryParse(request.Id, out Guid batchId))
        {
            errors.Add("Invalid Batch ID format.");
        }

        // Query database if ID is valid
        Batch? batch = null;
        if (errors.Count == 0)
        {
            batch = await _context.Batches.FirstOrDefaultAsync(b => b.Id == batchId);
            if (batch == null)
            {
                errors.Add("Batch not found.");
            }
        }

        // Return error response if any errors exist
        if (errors.Count > 0)
        {
            return new BatchResponse
            {
                Success = false,
                Error = { errors }
            };
        }

        // Return success response
        return new BatchResponse
        {
            Success = true,
            Data = new BatchData
            {
                Id = batch!.Id.ToString(),
                Title = batch.Title,
                StartYear = batch.StartYear
            }
        };
    }
}
