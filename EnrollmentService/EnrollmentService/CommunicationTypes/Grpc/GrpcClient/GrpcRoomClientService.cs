using Grpc.Net.Client;
using MajorService;

namespace EnrollmentService.CommunicationTypes.Grpc.GrpcClient
{
    public class GrpcRoomClientService
    {
        private readonly IConfiguration _configuration;
        private readonly GrpcRoom.GrpcRoomClient? _client;

        public GrpcRoomClientService(IConfiguration configuration)
        {
            _configuration = configuration;
            var majorServiceUrl = _configuration["GrpcSettings:MajorServiceUrl"];
            
            if (!string.IsNullOrEmpty(majorServiceUrl))
            {
                var httpHandler = new HttpClientHandler();
                // Return `true` to allow certificates that are untrusted/invalid
                httpHandler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                var channel = GrpcChannel.ForAddress(majorServiceUrl, new GrpcChannelOptions
                {
                    HttpHandler = httpHandler
                });
                _client = new GrpcRoom.GrpcRoomClient(channel);
            }
        }

        public async Task<RoomResponse> GetRoomByIdAsync(string id)
        {
            if (_client == null)
            {
                throw new InvalidOperationException("gRPC client for Major Service is not configured.");
            }

            try
            {
                var request = new RoomRequest { Id = id };
                return await _client.GetRoomByIdAsync(request);
            }
            catch (Exception ex)
            {
                return new RoomResponse
                {
                    Success = false,
                    Error = { $"gRPC call failed: {ex.Message}" }
                };
            }
        }
    }
}
