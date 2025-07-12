using Grpc.Core;
using EnrollmentService.Business.Services;
using EnrollmentService.DataAccess.Repositories;

namespace EnrollmentService.CommunicationTypes.Grpc.GrpcServer
{
    public class GrpcEnrollmentServerService : GrpcEnrollment.GrpcEnrollmentBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public GrpcEnrollmentServerService(IEnrollmentService enrollmentService, IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentService = enrollmentService;
            _enrollmentRepository = enrollmentRepository;
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
        }       
        
        public override async Task<GetFirstEnrollmentStatusResponse> GetFirstEnrollmentStatus(GetFirstEnrollmentStatusRequest request, ServerCallContext context)
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
    
public override async Task<GetAverageScoreResponse> GetAverageScore(GetAverageScoreRequest request, ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.AcademicClassId, out var academicClassId))
                {
                    return new GetAverageScoreResponse
                    {
                        Success = false,
                        AverageScore = 0,
                        Error = { "Invalid academic class ID format" }
                    };
                }

                // Fetch enrollments for the class
                var enrollments = await _enrollmentService.GetEnrollmentsByAcademicClassIdAsync(academicClassId);

                if (enrollments == null || !enrollments.Any())
                {
                    return new GetAverageScoreResponse
                    {
                        Success = false,
                        AverageScore = 0,
                        Error = { "No enrollments found for this class." }
                    };
                }

                // Safely handle nullable OverallScore values
                var validScores = enrollments
                    .Where(e => e.OverallScore.HasValue)
                    .Select(e => e.OverallScore!.Value)
                    .ToList();

                if (!validScores.Any())
                {
                    return new GetAverageScoreResponse
                    {
                        Success = false,
                        AverageScore = 0,
                        Error = { "No valid OverallScore values found for this class." }
                    };
                }

                double avg = validScores.Average();

                return new GetAverageScoreResponse
                {
                    Success = true,
                    AverageScore = avg
                };
            }
            catch (Exception ex)
            {
                return new GetAverageScoreResponse
                {
                    Success = false,
                    AverageScore = 0,
                    Error = { ex.Message }
                };
            }
        }

        public override async Task<GetPassFailCountResponse> GetPassFailCount(GetPassFailCountRequest request, ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.AcademicClassId, out var academicClassId))
                {
                    return new GetPassFailCountResponse
                    {
                        Success = false,
                        TotalPassed = 0,
                        TotalFailed = 0,
                        Error = { "Invalid academic class ID format" }
                    };
                }

                var enrollments = await _enrollmentService.GetEnrollmentsByAcademicClassIdAsync(academicClassId);

                if (enrollments == null || !enrollments.Any())
                {
                    return new GetPassFailCountResponse
                    {
                        Success = false,
                        TotalPassed = 0,
                        TotalFailed = 0,
                        Error = { "No enrollments found for this class." }
                    };
                }

                int totalPassed = enrollments.Count(e => e.OverallScore.HasValue && e.OverallScore.Value >= 5);
                int totalFailed = enrollments.Count(e => e.OverallScore.HasValue && e.OverallScore.Value < 5);

                return new GetPassFailCountResponse
                {
                    Success = true,
                    TotalPassed = totalPassed,
                    TotalFailed = totalFailed
                };
            }
            catch (Exception ex)
            {
                return new GetPassFailCountResponse
                {
                    Success = false,
                    TotalPassed = 0,
                    TotalFailed = 0,
                    Error = { ex.Message }
                };
            }
        }
    }
}
