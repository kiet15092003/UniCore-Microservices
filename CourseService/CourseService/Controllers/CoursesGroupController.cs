using CourseService.Business.Dtos.CoursesGroup;
using CourseService.Business.Services;
using CourseService.Middleware;
using CourseService.Utils.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace CourseService.Controllers
{
    [ApiController]
    [Route("api/c/[controller]")]
    public class CoursesGroupController : ControllerBase
    {
        private readonly ICoursesGroupService _coursesGroupService;

        public CoursesGroupController(ICoursesGroupService coursesGroupService)
        {
            _coursesGroupService = coursesGroupService;
        }   

        [HttpGet]
        public async Task<ApiResponse<CoursesGroupListResponse>> GetCoursesGroups(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int itemsPerPage = 10,
            [FromQuery] string orderBy = null,
            [FromQuery] bool isDesc = false)
        {
            var pagination = new Pagination { PageNumber = pageNumber, ItemsPerpage = itemsPerPage };
            var order = !string.IsNullOrEmpty(orderBy) ? new Order { By = orderBy, IsDesc = isDesc } : null;
            
            var result = await _coursesGroupService.GetCoursesGroupsByPaginationAsync(pagination, order);
            return ApiResponse<CoursesGroupListResponse>.SuccessResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<CoursesGroupReadDto>> GetCoursesGroupById(Guid id)
        {
            var coursesGroup = await _coursesGroupService.GetCoursesGroupByIdAsync(id);
            return ApiResponse<CoursesGroupReadDto>.SuccessResponse(coursesGroup);
        }
        [HttpPost]
        public async Task<ApiResponse<CoursesGroupReadDto>> CreateCoursesGroup([FromBody] CoursesGroupCreateDto coursesGroupCreateDto)
        {
            try
            {
                var createdCoursesGroup = await _coursesGroupService.CreateCoursesGroupAsync(coursesGroupCreateDto);
                return ApiResponse<CoursesGroupReadDto>.SuccessResponse(createdCoursesGroup);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<CoursesGroupReadDto>.ErrorResponse([ex.Message]);
            }
        }        
        [HttpPost("multiple")]
        public async Task<ApiResponse<IEnumerable<CoursesGroupReadDto>>> CreateMultipleCoursesGroups(
            [FromBody] List<CoursesGroupCreateDto> coursesGroupCreateDtos)
        {
            try
            {
                var createdCoursesGroups = await _coursesGroupService.CreateMultipleCoursesGroupsAsync(coursesGroupCreateDtos);
                return ApiResponse<IEnumerable<CoursesGroupReadDto>>.SuccessResponse(createdCoursesGroups);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<IEnumerable<CoursesGroupReadDto>>.ErrorResponse([ex.Message]);
            }
        }

        [HttpGet("major/{majorId}")]
        public async Task<ApiResponse<IEnumerable<CoursesGroupReadDto>>> GetCoursesGroupsByMajorId(Guid majorId)
        {
            var coursesGroups = await _coursesGroupService.GetCoursesGroupsByMajorIdAsync(majorId);
            return ApiResponse<IEnumerable<CoursesGroupReadDto>>.SuccessResponse(coursesGroups);
        }

        [HttpGet("open-for-all")]
        public async Task<ApiResponse<IEnumerable<CoursesGroupReadDto>>> GetCoursesGroupsWithAllCoursesOpenForAll()
        {
            var coursesGroups = await _coursesGroupService.GetCoursesGroupsWithAllCoursesOpenForAllAsync();
            return ApiResponse<IEnumerable<CoursesGroupReadDto>>.SuccessResponse(coursesGroups);
        }
    }
}