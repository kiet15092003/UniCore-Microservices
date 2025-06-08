using Grpc.Core;
using EnrollmentService.Business.Services;

namespace EnrollmentService.CommunicationTypes.Grpc.GrpcServer
{
    public class GrpcEnrollmentServerService : GrpcEnrollment.GrpcEnrollmentBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public GrpcEnrollmentServerService(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        public override async Task<EnrollmentCountResponse> GetEnrollmentCount(EnrollmentCountRequest request, ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.AcademicClassId, out var academicClassId))
                {
                    return new EnrollmentCountResponse
                    {
                        Success = false,
                        Count = 0,
                        Error = { "Invalid academic class ID format" }
                    };
                }

                var count = await _enrollmentService.GetEnrollmentCountByAcademicClassIdAsync(academicClassId);

                return new EnrollmentCountResponse
                {
                    Success = true,
                    Count = count,
                };
            }
            catch (Exception ex)
            {
                return new EnrollmentCountResponse
                {
                    Success = false,
                    Count = 0,
                    Error = { ex.Message }
                };
            }
        }
    }
}
