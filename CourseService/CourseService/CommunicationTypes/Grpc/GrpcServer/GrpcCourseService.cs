using Grpc.Core;
using CourseService;
using CourseService.Business.Services;
using System.Threading.Tasks;

namespace CourseService.CommunicationTypes.Grpc.GrpcServer
{
    public class GrpcCourseService : GrpcCourse.GrpcCourseBase
    {
        private readonly ICourseService _courseService;

        public GrpcCourseService(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public override async Task<GetCourseResponse> GetCourseById(GetCourseRequest request, ServerCallContext context)
        {
            var response = new GetCourseResponse();
            try
            {
                if (!Guid.TryParse(request.Id, out Guid courseId))
                {
                    response.Success = false;
                    response.Error.Add("Invalid course ID format.");
                    return response;
                }

                var courseDto = await _courseService.GetCourseByIdAsync(courseId);
                if (courseDto == null)
                {
                    response.Success = false;
                    response.Error.Add("Course not found.");
                    return response;
                }

                response.Success = true;
                response.Data = new CourseInfo
                {
                    Id = courseDto.Id.ToString(),
                    Code = courseDto.Code ?? string.Empty,
                    Name = courseDto.Name ?? string.Empty,
                    Description = courseDto.Description ?? string.Empty,
                    IsActive = courseDto.IsActive,
                    Credit = courseDto.Credit,
                    PracticePeriod = courseDto.PracticePeriod,
                    TheoryPeriod = courseDto.TheoryPeriod,
                    IsRequired = courseDto.IsRequired,
                    IsOpenForAll = courseDto.IsOpenForAll,
                    MinCreditRequired = courseDto.MinCreditRequired ?? 0
                };

                if (courseDto.MajorIds != null)
                    response.Data.MajorIds.AddRange(courseDto.MajorIds.Select(x => x.ToString()));
                if (courseDto.PreCourseIds != null)
                    response.Data.PreCourseIds.AddRange(courseDto.PreCourseIds.Select(x => x.ToString()));
                if (courseDto.ParallelCourseIds != null)
                    response.Data.ParallelCourseIds.AddRange(courseDto.ParallelCourseIds.Select(x => x.ToString()));
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error.Add(ex.Message);
            }
            return response;
        }
    }
}