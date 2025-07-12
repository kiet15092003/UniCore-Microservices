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
        public async Task<int> GetFirstEnrollmentStatusAsync(string academicClassId)
        {
            var grpcUrl = _configuration["GrpcSettings:EnrollmentServiceUrl"];

            using var channel = GrpcChannel.ForAddress(grpcUrl);
            var client = new GrpcEnrollment.GrpcEnrollmentClient(channel);

            try
            {
                var request = new GetFirstEnrollmentStatusRequest { AcademicClassId = academicClassId };
                var response = await client.GetFirstEnrollmentStatusAsync(request);

                return response.Status;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<double> GetAverageScoreAsync(string academicClassId)
        {
            var grpcUrl = _configuration["GrpcSettings:EnrollmentServiceUrl"];

            using var channel = GrpcChannel.ForAddress(grpcUrl);
            var client = new GrpcEnrollment.GrpcEnrollmentClient(channel);

            try
            {
                var request = new GetAverageScoreRequest { AcademicClassId = academicClassId };
                var response = await client.GetAverageScoreAsync(request);

                return response.Success ? response.AverageScore : 0.0;
            }
            catch (Exception)
            {
                return 0.0;
            }
        }

        public async Task<(int totalPassed, int totalFailed)> GetPassFailCountAsync(string academicClassId)
        {
            var grpcUrl = _configuration["GrpcSettings:EnrollmentServiceUrl"];

            using var channel = GrpcChannel.ForAddress(grpcUrl);
            var client = new GrpcEnrollment.GrpcEnrollmentClient(channel);

            try
            {
                var request = new GetPassFailCountRequest { AcademicClassId = academicClassId };
                var response = await client.GetPassFailCountAsync(request);

                return response.Success ? (response.TotalPassed, response.TotalFailed) : (0, 0);
            }
            catch (Exception)
            {
                return (0, 0);
            }
        }
    }
}
