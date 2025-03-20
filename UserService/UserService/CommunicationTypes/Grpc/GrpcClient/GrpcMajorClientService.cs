using Grpc.Net.Client;

namespace UserService.CommunicationTypes.Grpc.GrpcClient
{
    public class GrpcMajorClientService
    {
        private readonly IConfiguration _configuration;

        public GrpcMajorClientService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<MajorResponse> GetMajorByIdAsync(string id)
        {
            var grpcUrl = _configuration["GrpcSettings:MajorServiceUrl"];

            using var channel = GrpcChannel.ForAddress(grpcUrl);
            var client = new GrpcMajor.GrpcMajorClient(channel);

            try
            {
                var request = new MajorRequest { Id = id };
                return await client.GetMajorByIdAsync(request);
            }
            catch (Exception ex)
            {
                return new MajorResponse
                {
                    Success = false,
                    Error = { $"gRPC call failed: {ex.Message}" }
                };
            }
        }
    }
}
