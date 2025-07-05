using EnrollmentService.CommunicationTypes.Grpc.GrpcClient;
using EnrollmentService.CommunicationTypes.KafkaService.KafkaConsumer;
using EnrollmentService.CommunicationTypes.KafkaService.KafkaConsumer.Services;

namespace EnrollmentService.CommunicationTypes
{
    public static class CommunicationTypesRegistration
    {
        public static void AddCommunicationTypes(this IServiceCollection services)
        {
            services.AddScoped<IKafkaConsumerFunction, KafkaConsumerFunction>();
            
            // Register gRPC clients
            services.AddScoped<GrpcAcademicClassClientService>();
            services.AddScoped<GrpcStudentClientService>();
            services.AddScoped<GrpcRoomClientService>();
        }
    }
}
