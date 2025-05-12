using AutoMapper;
using CourseService.Business.Dtos.CoursesGroup;
using CourseService.DataAccess.Repositories;
using CourseService.Entities;
using CourseService.Utils.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Business.Services
{
    public class CoursesGroupSvc : ICoursesGroupService
    {
        private readonly ICoursesGroupRepository _coursesGroupRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public CoursesGroupSvc(
            ICoursesGroupRepository coursesGroupRepository,
            ICourseRepository courseRepository,
            IMapper mapper)
        {
            _coursesGroupRepository = coursesGroupRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        public async Task<CoursesGroupReadDto> CreateCoursesGroupAsync(CoursesGroupCreateDto coursesGroupCreateDto)
        {
            // First check if the group name is already in use for this major
            var groupNameExists = await _coursesGroupRepository.GroupNameExistsInMajorAsync(
                coursesGroupCreateDto.GroupName,
                coursesGroupCreateDto.MajorId);
                
            if (groupNameExists)
            {
                throw new InvalidOperationException($"A course group with name '{coursesGroupCreateDto.GroupName}' already exists in this major.");
            }
            
            var coursesGroup = _mapper.Map<CoursesGroup>(coursesGroupCreateDto);
            coursesGroup.CreatedAt = DateTime.Now;
            coursesGroup.UpdatedAt = DateTime.Now;
            
            // Store CourseIds directly
            coursesGroup.CourseIds = coursesGroupCreateDto.CourseIds?.ToList() ?? new List<Guid>();
            
            // Validate course IDs exist if needed
            if (coursesGroupCreateDto.CourseIds != null && coursesGroupCreateDto.CourseIds.Any())
            {
                var validCourseIds = new List<Guid>();
                var validCourses = new List<Entities.Course>();
                
                foreach (var courseId in coursesGroupCreateDto.CourseIds)
                {
                    var course = await _courseRepository.GetCourseByIdAsync(courseId);
                    if (course != null)
                    {
                        validCourseIds.Add(courseId);
                        validCourses.Add(course);
                    }
                }
                // Only keep valid course IDs
                coursesGroup.CourseIds = validCourseIds;
            }
            
            var createdCoursesGroup = await _coursesGroupRepository.CreateCoursesGroupAsync(coursesGroup);
            var result = _mapper.Map<CoursesGroupReadDto>(createdCoursesGroup);
              // Populate courses data
            if (result.CourseIds.Any())
            {
                var courses = await _courseRepository.GetCoursesByIdsAsync(result.CourseIds);
                result.Courses = _mapper.Map<List<Dtos.Course.CourseReadDto>>(courses);
            }
            
            return result;
        }

        public async Task<IEnumerable<CoursesGroupReadDto>> CreateMultipleCoursesGroupsAsync(IEnumerable<CoursesGroupCreateDto> coursesGroupCreateDtos)
        {
            // First validate all group names are unique within their respective majors
            var groupNamesMajorIds = new Dictionary<(string, Guid), bool>();
            foreach (var dto in coursesGroupCreateDtos)
            {
                var key = (dto.GroupName, dto.MajorId);
                
                if (groupNamesMajorIds.ContainsKey(key))
                {
                    throw new InvalidOperationException($"Duplicate course group name '{dto.GroupName}' found in the same major.");
                }
                
                groupNamesMajorIds[key] = true;
                
                // Also check against existing records in the database
                var groupNameExists = await _coursesGroupRepository.GroupNameExistsInMajorAsync(
                    dto.GroupName,
                    dto.MajorId);
                    
                if (groupNameExists)
                {
                    throw new InvalidOperationException($"A course group with name '{dto.GroupName}' already exists in the specified major.");
                }
            }
            
            // First collect all course IDs to validate they exist
            var allCourseIds = coursesGroupCreateDtos
                .SelectMany(cg => cg.CourseIds ?? Enumerable.Empty<Guid>())
                .Distinct()
                .ToList();
            
            // Validate courses exist
            Dictionary<Guid, bool> validCourseIds = new Dictionary<Guid, bool>();
            Dictionary<Guid, Entities.Course> validCourses = new Dictionary<Guid, Entities.Course>();
            
            if (allCourseIds.Any())
            {
                foreach (var courseId in allCourseIds)
                {
                    var course = await _courseRepository.GetCourseByIdAsync(courseId);
                    validCourseIds[courseId] = course != null;
                    if (course != null)
                    {
                        validCourses[courseId] = course;
                    }
                }
            }
            
            // Map and prepare course groups
            var coursesGroups = new List<CoursesGroup>();
            var currentTime = DateTime.Now;
            
            foreach (var dto in coursesGroupCreateDtos)
            {
                var coursesGroup = _mapper.Map<CoursesGroup>(dto);
                coursesGroup.CreatedAt = currentTime;
                coursesGroup.UpdatedAt = currentTime;
                
                // Store valid CourseIds directly
                coursesGroup.CourseIds = dto.CourseIds?
                    .Where(id => validCourseIds.ContainsKey(id) && validCourseIds[id])
                    .ToList() ?? new List<Guid>();
                
                coursesGroups.Add(coursesGroup);
            }
            
            // Save all course groups
            var createdCoursesGroups = await _coursesGroupRepository.CreateMultipleCoursesGroupsAsync(coursesGroups);
            var result = _mapper.Map<IEnumerable<CoursesGroupReadDto>>(createdCoursesGroups).ToList();
              // Get all course IDs from all groups
            var allGroupCourseIds = result
                .SelectMany(g => g.CourseIds)
                .Distinct()
                .ToList();
                
            // Create a lookup dictionary for quick access from previously validated courses
            var courseLookup = validCourses;
            
            // Populate courses for each group
            foreach (var groupDto in result)
            {
                if (groupDto.CourseIds.Any())
                {
                    groupDto.Courses = new List<Dtos.Course.CourseReadDto>();
                    foreach (var courseId in groupDto.CourseIds)
                    {
                        if (courseLookup.ContainsKey(courseId))
                        {
                            groupDto.Courses.Add(_mapper.Map<Dtos.Course.CourseReadDto>(courseLookup[courseId]));
                        }
                    }
                }
            }
            
            return result;
        }

        public async Task<CoursesGroupReadDto?> GetCoursesGroupByIdAsync(Guid id)
        {
            var coursesGroup = await _coursesGroupRepository.GetCoursesGroupByIdAsync(id);
            if (coursesGroup == null)
            {
                return null;
            }
            
            var result = _mapper.Map<CoursesGroupReadDto>(coursesGroup);
            
            // Ensure CourseIds is always initialized
            if (result.CourseIds == null)
            {
                result.CourseIds = new List<Guid>();
            }
              // Fetch the full course data for each course ID
            if (result.CourseIds.Any())
            {
                var courses = await _courseRepository.GetCoursesByIdsAsync(result.CourseIds);
                result.Courses = _mapper.Map<List<Dtos.Course.CourseReadDto>>(courses);
            }
            
            return result;
        }

        public async Task<CoursesGroupListResponse> GetCoursesGroupsByPaginationAsync(Pagination pagination, Order? order)
        {
            var result = await _coursesGroupRepository.GetAllCoursesGroupsPaginationAsync(pagination, order);
            
            var response = _mapper.Map<CoursesGroupListResponse>(result);
              // Ensure CourseIds is always initialized for all items
            foreach (var item in response.Data)
            {
                if (item.CourseIds == null)
                {
                    item.CourseIds = new List<Guid>();
                }
            }
            
            // Get all course IDs from all groups
            var allCourseIds = response.Data
                .SelectMany(g => g.CourseIds)
                .Distinct()
                .ToList();
                
            // Fetch all courses in a single query
            var allCourses = allCourseIds.Any() 
                ? await _courseRepository.GetCoursesByIdsAsync(allCourseIds)
                : new List<Entities.Course>();
                
            // Create a lookup dictionary for quick access
            var courseLookup = allCourses.ToDictionary(c => c.Id, c => c);
            
            // Populate courses for each group
            foreach (var item in response.Data)
            {
                if (item.CourseIds.Any())
                {
                    item.Courses = new List<Dtos.Course.CourseReadDto>();
                    foreach (var courseId in item.CourseIds)
                    {
                        if (courseLookup.TryGetValue(courseId, out var course))
                        {
                            item.Courses.Add(_mapper.Map<Dtos.Course.CourseReadDto>(course));
                        }
                    }
                }
            }
            
            return response;
        }

        public async Task<IEnumerable<CoursesGroupReadDto>> GetCoursesGroupsByMajorIdAsync(Guid majorId)
        {
            var coursesGroups = await _coursesGroupRepository.GetCoursesGroupsByMajorIdAsync(majorId);
            
            var result = _mapper.Map<IEnumerable<CoursesGroupReadDto>>(coursesGroups).ToList();
              // Ensure CourseIds is always initialized for all items
            foreach (var item in result)
            {
                if (item.CourseIds == null)
                {
                    item.CourseIds = new List<Guid>();
                }
            }
            
            // Get all course IDs from all groups
            var allCourseIds = result
                .SelectMany(g => g.CourseIds)
                .Distinct()
                .ToList();
                
            // Fetch all courses in a single query
            var allCourses = allCourseIds.Any() 
                ? await _courseRepository.GetCoursesByIdsAsync(allCourseIds)
                : new List<Entities.Course>();
                
            // Create a lookup dictionary for quick access
            var courseLookup = allCourses.ToDictionary(c => c.Id, c => c);
            
            // Populate courses for each group
            foreach (var item in result)
            {
                if (item.CourseIds.Any())
                {
                    item.Courses = new List<Dtos.Course.CourseReadDto>();
                    foreach (var courseId in item.CourseIds)
                    {
                        if (courseLookup.TryGetValue(courseId, out var course))
                        {
                            item.Courses.Add(_mapper.Map<Dtos.Course.CourseReadDto>(course));
                        }
                    }
                }
            }
            
            return result;
        }
    }
}