using AutoMapper;
using CourseService.Business.Dtos.Course;
using CourseService.CommunicationTypes.Grpc.GrpcClient;
using CourseService.DataAccess.Repositories;
using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Services
{
    public class CourseSvc : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly GrpcMajorClientService _grpcClient;

        public CourseSvc(
            ICourseRepository courseRepository, 
            IMapper mapper,
            GrpcMajorClientService grpcClient)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
            _grpcClient = grpcClient;
        }
        public async Task<CourseReadDto> CreateCourseAsync(CourseCreateDto courseCreateDto)
        {
            if (courseCreateDto.MajorId.HasValue)
            {
                var major = await _grpcClient.GetMajorByIdAsync(courseCreateDto.MajorId.Value.ToString());
                if (!major.Success)
                {
                    throw new KeyNotFoundException("Major not found");
                }
            }

            var course = _mapper.Map<Course>(courseCreateDto);
            var createdCourse = await _courseRepository.CreateCourseAsync(course);
            return _mapper.Map<CourseReadDto>(createdCourse);
        }

        public async Task<CourseListResponse> GetProductByPagination(Pagination pagination, CourseListFilterParams courseListFilterParams, Order? order)
        {
            var result = await _courseRepository.GetAllCoursesPaginationAsync(pagination, courseListFilterParams, order);
            return _mapper.Map<CourseListResponse>(result); 
        }

        public async Task<CourseReadDto> UpdateCourseAsync(Guid id, CourseUpdateDto courseUpdateDto)
        {
            if (courseUpdateDto.MajorId.HasValue)
            {
                
                var major = await _grpcClient.GetMajorByIdAsync(courseUpdateDto.MajorId.Value.ToString());
                if (!major.Success)
                {
                    throw new KeyNotFoundException("Major not found");
                }
            }
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null)
            {
                throw new KeyNotFoundException("Course not found");
            }

            _mapper.Map(courseUpdateDto, course);
            var updatedCourse = await _courseRepository.UpdateCourseAsync(course);
            return _mapper.Map<CourseReadDto>(updatedCourse);
        }
        
        public async Task<bool> DeleteCourseAsync(Guid id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null)
            {
                throw new KeyNotFoundException("Course not found");
            }

            return await _courseRepository.DeleteCourseAsync(id);
        }
    }
}
