using AutoMapper;
using CourseService.Business.Dtos.Course;
using CourseService.CommunicationTypes.Grpc.GrpcClient;
using CourseService.DataAccess.Repositories;
using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;
using System.Collections.Generic;
using System.Linq;

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
            // Verify that all provided major IDs exist
            var majors = new List<MajorData>();
            
            if (courseCreateDto.MajorIds != null && courseCreateDto.MajorIds.Length > 0)
            {
                foreach (var majorId in courseCreateDto.MajorIds)
                {
                    var majorResponse = await _grpcClient.GetMajorByIdAsync(majorId.ToString());
                    if (!majorResponse.Success)
                    {
                        throw new KeyNotFoundException($"Major with ID {majorId} not found");
                    }
                    
                    if (majorResponse.Data != null)
                    {
                        majors.Add(majorResponse.Data);
                    }
                }
            }

            var course = _mapper.Map<Course>(courseCreateDto);
            var createdCourse = await _courseRepository.CreateCourseAsync(course);
            var courseReadDto = _mapper.Map<CourseReadDto>(createdCourse);
            
            // Set majors data to the courseReadDto
            if (majors.Count > 0)
            {
                courseReadDto.Majors = majors.ToArray();
                
                // Calculate cost based on the first major (or you could implement a different logic)
                double costPerCredit = majors[0].CostPerCredit > 0 ? majors[0].CostPerCredit : DEFAULT_COST_PER_CREDIT;
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
            
            // Populate majors data for each course in the list
            foreach (var course in response.Data)
            {
                if (course.MajorIds != null && course.MajorIds.Length > 0)
                {
                    var majors = new List<MajorData>();
                    
                    foreach (var majorId in course.MajorIds)
                    {
                        var majorResponse = await _grpcClient.GetMajorByIdAsync(majorId.ToString());
                        if (majorResponse.Success && majorResponse.Data != null)
                        {
                            majors.Add(majorResponse.Data);
                        }
                    }
                    
                    if (majors.Count > 0)
                    {
                        course.Majors = majors.ToArray();
                        
                        // Calculate cost based on the first major
                        double costPerCredit = majors[0].CostPerCredit > 0 ? majors[0].CostPerCredit : DEFAULT_COST_PER_CREDIT;
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
            // Verify that all provided major IDs exist
            var majors = new List<MajorData>();
            
            if (courseUpdateDto.MajorIds != null && courseUpdateDto.MajorIds.Length > 0)
            {
                foreach (var majorId in courseUpdateDto.MajorIds)
                {
                    var majorResponse = await _grpcClient.GetMajorByIdAsync(majorId.ToString());
                    if (!majorResponse.Success)
                    {
                        throw new KeyNotFoundException($"Major with ID {majorId} not found");
                    }
                    
                    if (majorResponse.Data != null)
                    {
                        majors.Add(majorResponse.Data);
                    }
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
            
            // Set majors data and calculate cost
            if (majors.Count > 0)
            {
                courseReadDto.Majors = majors.ToArray();
                
                // Calculate cost based on the first major
                double costPerCredit = majors[0].CostPerCredit > 0 ? majors[0].CostPerCredit : DEFAULT_COST_PER_CREDIT;
                courseReadDto.Cost = courseReadDto.Credit * costPerCredit;
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
            
            // Set majors data and calculate cost
            var majors = new List<MajorData>();
            
            if (course.MajorIds != null && course.MajorIds.Length > 0)
            {
                foreach (var majorId in course.MajorIds)
                {
                    var majorResponse = await _grpcClient.GetMajorByIdAsync(majorId.ToString());
                    if (majorResponse.Success && majorResponse.Data != null)
                    {
                        majors.Add(majorResponse.Data);
                    }
                }
                
                if (majors.Count > 0)
                {
                    courseReadDto.Majors = majors.ToArray();
                    
                    // Calculate cost based on the first major
                    double costPerCredit = majors[0].CostPerCredit > 0 ? majors[0].CostPerCredit : DEFAULT_COST_PER_CREDIT;
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
          public async Task<List<CourseReadDto>> GetCoursesByMajorIdAsync(Guid majorId)
        {
            // Verify that the major exists
            var majorResponse = await _grpcClient.GetMajorByIdAsync(majorId.ToString());
            if (!majorResponse.Success)
            {
                throw new KeyNotFoundException($"Major with ID {majorId} not found");
            }
            
            // Get all courses
            var pagination = new Pagination { PageNumber = 1, ItemsPerpage = 1000 }; // Large page size to get all
            var filter = new CourseListFilterParams { IsActive = true }; // Get only active courses
            var coursesResult = await _courseRepository.GetAllCoursesPaginationAsync(pagination, filter, null);
              // Filter courses that have the specified majorId or are open for all
            var filteredCourses = coursesResult.Data
                .Where(c => c.IsOpenForAll || (c.MajorIds != null && c.MajorIds.Contains(majorId)))
                .ToList();
            
            // Map to DTOs
            var courseReadDtos = _mapper.Map<List<CourseReadDto>>(filteredCourses);
            
            // Set majors data for each course
            foreach (var courseDto in courseReadDtos)
            {
                if (courseDto.MajorIds != null && courseDto.MajorIds.Length > 0)
                {
                    var majors = new List<MajorData>();
                    
                    foreach (var id in courseDto.MajorIds)
                    {
                        // For the requested majorId, we already have the data
                        if (id == majorId)
                        {
                            majors.Add(majorResponse.Data);
                        }
                        else
                        {
                            // For other majorIds, fetch their data
                            var otherMajorResponse = await _grpcClient.GetMajorByIdAsync(id.ToString());
                            if (otherMajorResponse.Success && otherMajorResponse.Data != null)
                            {
                                majors.Add(otherMajorResponse.Data);
                            }
                        }
                    }
                    
                    if (majors.Count > 0)
                    {
                        courseDto.Majors = majors.ToArray();
                        
                        // Calculate cost based on the first major
                        double costPerCredit = majors[0].CostPerCredit > 0 ? majors[0].CostPerCredit : DEFAULT_COST_PER_CREDIT;
                        courseDto.Cost = courseDto.Credit * costPerCredit;
                    }
                    else
                    {
                        courseDto.Cost = courseDto.Credit * DEFAULT_COST_PER_CREDIT;
                    }
                }
                else
                {
                    courseDto.Cost = courseDto.Credit * DEFAULT_COST_PER_CREDIT;
                }
            }
            
            return courseReadDtos;
        }
    }
}
