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

        public override async Task<GetEnrollmentsByClassIdResponse> GetEnrollmentsByClassId(GetEnrollmentsByClassIdRequest request, ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.AcademicClassId, out var academicClassId))
                {
                    return new GetEnrollmentsByClassIdResponse
                    {
                        Success = false,
                        Error = { "Invalid academic class ID format" }
                    };
                }

                var enrollments = await _enrollmentService.GetEnrollmentsByAcademicClassIdAsync(academicClassId);

                var enrollmentDtos = enrollments.Select(e => new EnrollmentDto
                {
                    Id = e.Id.ToString(),
                    Status = e.Status,
                    StudentId = e.StudentId.ToString(),
                    AcademicClassId = e.AcademicClassId.ToString()
                }).ToList();

                return new GetEnrollmentsByClassIdResponse
                {
                    Success = true,
                    Enrollments = { enrollmentDtos }
                };
            }
            catch (Exception ex)
            {
                return new GetEnrollmentsByClassIdResponse
                {
                    Success = false,
                    Error = { ex.Message }
                };
            }
        }        public override async Task<GetFirstEnrollmentStatusResponse> GetFirstEnrollmentStatus(GetFirstEnrollmentStatusRequest request, ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.AcademicClassId, out var academicClassId))
                {
                    return new GetFirstEnrollmentStatusResponse
                    {
                        Success = false,
                        Status = 0,
                        Error = { "Invalid academic class ID format" }
                    };
                }

                var firstEnrollmentStatus = await _enrollmentService.GetFirstEnrollmentStatusByAcademicClassIdAsync(academicClassId);

                if (!firstEnrollmentStatus.HasValue)
                {
                    return new GetFirstEnrollmentStatusResponse
                    {
                        Success = true,
                        Status = 0,
                    };
                }

                return new GetFirstEnrollmentStatusResponse
                {
                    Success = true,
                    Status = firstEnrollmentStatus.Value,
                };
            }
            catch (Exception ex)
            {
                return new GetFirstEnrollmentStatusResponse
                {
                    Success = false,
                    Status = 0,
                    Error = { ex.Message }
                };
            }
        }
    }
}
