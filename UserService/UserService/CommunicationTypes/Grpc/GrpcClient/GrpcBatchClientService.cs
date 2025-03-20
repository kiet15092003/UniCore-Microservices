using Grpc.Net.Client;

namespace UserService.CommunicationTypes.Grpc.GrpcClient
{
    public class GrpcBatchClientService
    {
        private readonly IConfiguration _configuration;

        public GrpcBatchClientService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<BatchResponse> GetBatchByIdAsync(string id)
        {
            var grpcUrl = _configuration["GrpcSettings:StudentServiceUrl"];

            using var channel = GrpcChannel.ForAddress(grpcUrl);
            var client = new GrpcBatch.GrpcBatchClient(channel);

            try
            {
                var request = new BatchRequest { Id = id };
                return await client.GetBatchByIdAsync(request);
            }
            catch (Exception ex)
            {
                return new BatchResponse
                {
                    Success = false,
                    Error = { $"gRPC call failed: {ex.Message}" }
                };
            }
        }
    }
}
