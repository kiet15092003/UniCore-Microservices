using Grpc.Core;
using UserService.Business.Services.BatchService;
using System.Globalization;

namespace UserService.CommunicationTypes.Grpc.GrpcServer
{
    public class GrpcBatchService : GrpcBatch.GrpcBatchBase
    {
        private readonly IBatchService _batchService;
        private readonly ILogger<GrpcBatchService> _logger;

        public GrpcBatchService(
            IBatchService batchService,
            ILogger<GrpcBatchService> logger)
        {
            _batchService = batchService;
            _logger = logger;
        }

        public override async Task<BatchResponse> GetBatchById(BatchRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"gRPC call: GetBatchById with ID {request.Id}");

            if (string.IsNullOrWhiteSpace(request.Id) || !Guid.TryParse(request.Id, out Guid batchId))
            {
                return new BatchResponse
                {
                    Success = false,
                    Error = { "Invalid batch ID format" }
                };
            }

            try
            {
                var batch = await _batchService.GetBatchById(batchId);

                if (batch == null)
                {
                    return new BatchResponse
                    {
                        Success = false,
                        Error = { $"Batch with ID {request.Id} not found" }
                    };
                }

                return new BatchResponse
                {
                    Success = true,
                    Data = new BatchData
                    {
                        Id = batch.Id.ToString(),
                        Title = batch.Title,
                        StartYear = batch.StartYear,
                        CreatedAt = batch.CreatedAt.ToString("o", CultureInfo.InvariantCulture),
                        UpdatedAt = batch.UpdatedAt.ToString("o", CultureInfo.InvariantCulture)
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving batch with ID {request.Id}");
                return new BatchResponse
                {
                    Success = false,
                    Error = { $"Error retrieving batch: {ex.Message}" }
                };
            }
        }
    }
}
