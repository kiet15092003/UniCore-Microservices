using Grpc.Net.Client;
using MajorService;

namespace CourseService.CommunicationTypes.Grpc.GrpcClient
{
    public class GrpcRoomClientService
    {
        private readonly IConfiguration _configuration;

        public GrpcRoomClientService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<RoomResponse> GetRoomByIdAsync(string id)
        {
            var grpcUrl = _configuration["GrpcSettings:RoomServiceUrl"];

            using var channel = GrpcChannel.ForAddress(grpcUrl);
            var client = new GrpcRoom.GrpcRoomClient(channel);

            try
            {
                var request = new RoomRequest { Id = id };
                return await client.GetRoomByIdAsync(request);
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
