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
        private const double DEFAULT_COST_PER_CREDIT = 500000; // Default cost per credit if Major service doesn't provide one

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
            var major = await _grpcClient.GetMajorByIdAsync(courseCreateDto.MajorId.ToString());
            if (!major.Success)
            {
                throw new KeyNotFoundException("Major not found");
            }

            var course = _mapper.Map<Course>(courseCreateDto);
            var createdCourse = await _courseRepository.CreateCourseAsync(course);
            var courseReadDto = _mapper.Map<CourseReadDto>(createdCourse);
            
            // Calculate cost based on course credits and major's costPerCredit
            if (major.Data != null)
            {
                double costPerCredit = major.Data.CostPerCredit > 0 ? major.Data.CostPerCredit : DEFAULT_COST_PER_CREDIT;
                courseReadDto.Cost = courseReadDto.Credit * costPerCredit;
            }
            else
            {
                courseReadDto.Cost = courseReadDto.Credit * DEFAULT_COST_PER_CREDIT;
            }
            
            return courseReadDto;
        }

        public async Task<CourseListResponse> GetProductByPagination(Pagination pagination, CourseListFilterParams courseListFilterParams, Order? order)
        {
            var result = await _courseRepository.GetAllCoursesPaginationAsync(pagination, courseListFilterParams, order);
            var response = _mapper.Map<CourseListResponse>(result);
            
            // Calculate cost for each course based on major's costPerCredit
            foreach (var course in response.Data)
            {
                if (course.MajorId.HasValue)
                {
                    var major = await _grpcClient.GetMajorByIdAsync(course.MajorId.Value.ToString());
                    if (major.Success && major.Data != null)
                    {
                        double costPerCredit = major.Data.CostPerCredit > 0 ? major.Data.CostPerCredit : DEFAULT_COST_PER_CREDIT;
                        course.Cost = course.Credit * costPerCredit;
                    }
                    else
                    {
                        course.Cost = course.Credit * DEFAULT_COST_PER_CREDIT;
                    }
                }
                else
                {
                    course.Cost = course.Credit * DEFAULT_COST_PER_CREDIT;
                }
            }
            
            return response;
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
            var courseReadDto = _mapper.Map<CourseReadDto>(updatedCourse);
            
            // Calculate cost
            if (courseReadDto.MajorId.HasValue)
            {
                var major = await _grpcClient.GetMajorByIdAsync(courseReadDto.MajorId.Value.ToString());
                if (major.Success && major.Data != null)
                {
                    double costPerCredit = major.Data.CostPerCredit > 0 ? major.Data.CostPerCredit : DEFAULT_COST_PER_CREDIT;
                    courseReadDto.Cost = courseReadDto.Credit * costPerCredit;
                }
                else
                {
                    courseReadDto.Cost = courseReadDto.Credit * DEFAULT_COST_PER_CREDIT;
                }
            }
            else
            {
                courseReadDto.Cost = courseReadDto.Credit * DEFAULT_COST_PER_CREDIT;
            }
            
            return courseReadDto;
        }
        
        public async Task<CourseReadDto> DeactivateCourseAsync(Guid id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null)
            {
                throw new KeyNotFoundException("Course not found");
            }

            course.IsActive = false;
            var updatedCourse = await _courseRepository.UpdateCourseAsync(course);
            var courseReadDto = _mapper.Map<CourseReadDto>(updatedCourse);
            
            // Calculate cost
            if (courseReadDto.MajorId.HasValue)
            {
                var major = await _grpcClient.GetMajorByIdAsync(courseReadDto.MajorId.Value.ToString());
                if (major.Success && major.Data != null)
                {
                    double costPerCredit = major.Data.CostPerCredit > 0 ? major.Data.CostPerCredit : DEFAULT_COST_PER_CREDIT;
                    courseReadDto.Cost = courseReadDto.Credit * costPerCredit;
                }
                else
                {
                    courseReadDto.Cost = courseReadDto.Credit * DEFAULT_COST_PER_CREDIT;
                }
            }
            else
            {
                courseReadDto.Cost = courseReadDto.Credit * DEFAULT_COST_PER_CREDIT;
            }
            
            return courseReadDto;
        }
    }
}
