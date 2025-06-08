using Grpc.Net.Client;

namespace CourseService.CommunicationTypes.Grpc.GrpcClient
{
    public class GrpcBatchClientService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GrpcBatchClientService> _logger;

        public GrpcBatchClientService(IConfiguration configuration, ILogger<GrpcBatchClientService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<BatchResponse> GetBatchByIdAsync(string id)
        {
            var grpcUrl = _configuration["GrpcSettings:UserServiceUrl"];
            _logger.LogInformation($"Calling gRPC GetBatchById with ID {id} at URL {grpcUrl}");

            using var channel = GrpcChannel.ForAddress(grpcUrl);
            var client = new GrpcBatch.GrpcBatchClient(channel);

            try
            {
                var request = new BatchRequest { Id = id };
                return await client.GetBatchByIdAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"gRPC call to GetBatchById failed for ID {id}");
                return new BatchResponse
                {
                    Success = false,
                    Error = { $"gRPC call failed: {ex.Message}" }
                };
            }
        }

        public async Task<List<BatchData>> GetBatchesByIdsAsync(List<string> ids)
        {
            var result = new List<BatchData>();
            
            foreach (var id in ids)
            {
                var response = await GetBatchByIdAsync(id);
                if (response.Success && response.Data != null)
                {
                    result.Add(response.Data);
                }
            }
            
            return result;
        }
    }
}
