using Grpc.Net.Client;

namespace UserService.CommunicationTypes.Grpc.GrpcClient
{
    public class GrpcDepartmentClientService
    {
        private readonly IConfiguration _configuration;

        public GrpcDepartmentClientService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<DepartmentResponse> GetDepartmentByIdAsync(string id)
        {
            var grpcUrl = _configuration["GrpcSettings:DepartmentServiceUrl"];

            using var channel = GrpcChannel.ForAddress(grpcUrl);
            var client = new GrpcDepartment.GrpcDepartmentClient(channel);

            try
            {
                var request = new DepartmentRequest { Id = id };
                return await client.GetDepartmentByIdAsync(request);
            }
            catch (Exception ex)
            {
                return new DepartmentResponse
                {
                    Success = false,
                    Error = { $"gRPC call failed: {ex.Message}" }
                };
            }
        }
    }
} 