using EnrollmentService;
using Grpc.Net.Client;

namespace CourseService.CommunicationTypes.Grpc.GrpcClient
{
    public class GrpcEnrollmentClientService
    {
        private readonly IConfiguration _configuration;

        public GrpcEnrollmentClientService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<EnrollmentCountResponse> GetEnrollmentCountAsync(string academicClassId)
        {
            var grpcUrl = _configuration["GrpcSettings:EnrollmentServiceUrl"];

            using var channel = GrpcChannel.ForAddress(grpcUrl);
            var client = new GrpcEnrollment.GrpcEnrollmentClient(channel);

            try
            {
                var request = new EnrollmentCountRequest { AcademicClassId = academicClassId };
                return await client.GetEnrollmentCountAsync(request);
            }
            catch (Exception ex)
            {
                return new EnrollmentCountResponse
                {
                    Success = false,
                    Count = 0,
                    Error = { $"gRPC call failed: {ex.Message}" }
                };
            }
        }
    }
}
