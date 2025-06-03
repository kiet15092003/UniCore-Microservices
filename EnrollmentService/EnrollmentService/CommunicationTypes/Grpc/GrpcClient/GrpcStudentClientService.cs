using EnrollmentService;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace EnrollmentService.CommunicationTypes.Grpc.GrpcClient
{    public class GrpcStudentClientService
    {
        private readonly IConfiguration _configuration;
        private readonly GrpcStudent.GrpcStudentClient? _client;

        public GrpcStudentClientService(IConfiguration configuration)
        {
            _configuration = configuration;
            var userServiceUrl = _configuration["GrpcServices:UserServiceUrl"];
            
            if (!string.IsNullOrEmpty(userServiceUrl))
            {
                var httpHandler = new HttpClientHandler();
                // Return `true` to allow certificates that are untrusted/invalid
                httpHandler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                var channel = GrpcChannel.ForAddress(userServiceUrl, new GrpcChannelOptions
                {
                    HttpHandler = httpHandler
                });
                _client = new GrpcStudent.GrpcStudentClient(channel);
            }
        }

        public async Task<StudentResponse> GetStudentById(string id)
        {
            if (_client == null)
            {
                throw new InvalidOperationException("gRPC client for User Service is not configured.");
            }

            var request = new StudentRequest { Id = id };
            return await _client.GetStudentByIdAsync(request);
        }
    }
}
