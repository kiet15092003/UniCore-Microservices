using EnrollmentService;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace EnrollmentService.CommunicationTypes.Grpc.GrpcClient
{    public class GrpcAcademicClassClientService
    {
        private readonly IConfiguration _configuration;
        private readonly GrpcAcademicClass.GrpcAcademicClassClient? _client;

        public GrpcAcademicClassClientService(IConfiguration configuration)
        {
            _configuration = configuration;
            var courseServiceUrl = _configuration["GrpcServices:CourseServiceUrl"];
            
            if (!string.IsNullOrEmpty(courseServiceUrl))
            {
                var httpHandler = new HttpClientHandler();
                // Return `true` to allow certificates that are untrusted/invalid
                httpHandler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                var channel = GrpcChannel.ForAddress(courseServiceUrl, new GrpcChannelOptions
                {
                    HttpHandler = httpHandler
                });
                _client = new GrpcAcademicClass.GrpcAcademicClassClient(channel);
            }
        }

        public async Task<AcademicClassResponse> GetAcademicClassById(string id)
        {
            if (_client == null)
            {
                throw new InvalidOperationException("gRPC client for Course Service is not configured.");
            }

            var request = new AcademicClassRequest { Id = id };
            return await _client.GetAcademicClassByIdAsync(request);
        }
    }
}
