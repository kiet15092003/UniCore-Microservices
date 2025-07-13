using AutoMapper;
using CourseService.Business.Dtos.TrainingRoadmap;
using CourseService.CommunicationTypes.Grpc.GrpcClient;
using CourseService.DataAccess.Repositories;
using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;
using System.Collections.Generic;

namespace CourseService.Business.Services
{    public class TrainingRoadmapSvc : ITrainingRoadmapService
    {
        private readonly ITrainingRoadmapRepository _trainingRoadmapRepository;
        private readonly ICoursesGroupRepository _coursesGroupRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly GrpcMajorClientService _grpcClient;
        private readonly GrpcBatchClientService _grpcBatchClient;
        private readonly ILogger<TrainingRoadmapSvc> _logger;        
        public TrainingRoadmapSvc(
            ITrainingRoadmapRepository trainingRoadmapRepository,
            ICoursesGroupRepository coursesGroupRepository,
            ICourseRepository courseRepository,
            IMapper mapper,
            GrpcMajorClientService grpcClient,
            GrpcBatchClientService grpcBatchClient,
            ILogger<TrainingRoadmapSvc> logger)
        {
            _trainingRoadmapRepository = trainingRoadmapRepository;
            _coursesGroupRepository = coursesGroupRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
            _grpcClient = grpcClient;
            _grpcBatchClient = grpcBatchClient;
            _logger = logger;
        }

        public async Task<TrainingRoadmapReadDto> CreateTrainingRoadmapAsync(TrainingRoadmapCreateDto createDto)
        {
            // Map DTO to entity
            var trainingRoadmap = _mapper.Map<TrainingRoadmap>(createDto);
            trainingRoadmap.CreatedAt = DateTime.Now;
            
            // Call repository to create the roadmap (code will be generated in the repository)
            var result = await _trainingRoadmapRepository.CreateTrainingRoadmapAsync(trainingRoadmap);
              // Map the result back to DTO
            var roadmapDto = _mapper.Map<TrainingRoadmapReadDto>(result);
            
            // Populate course group credits
            await PopulateCoursesGroupCreditsAsync(roadmapDto);
            
            // Get major information from gRPC service
            if (roadmapDto.MajorId.HasValue && roadmapDto.MajorId.Value != Guid.Empty)
            {
                var major = await _grpcClient.GetMajorByIdAsync(roadmapDto.MajorId.Value.ToString());
                if (major.Success && major.Data != null)
                {
                    roadmapDto.MajorData = major.Data;
                }
            }

            // Get batch information for BatchIds
            if (roadmapDto.BatchIds != null && roadmapDto.BatchIds.Any())
            {
                roadmapDto.BatchDatas = await GetBatchDataForIdsAsync(roadmapDto.BatchIds);
            }
            
            return roadmapDto;
        }        
        public async Task<TrainingRoadmapReadDto> DeactivateTrainingRoadmapAsync(Guid id)
        {
            var roadmap = await _trainingRoadmapRepository.GetTrainingRoadmapByIdAsync(id);
            if (roadmap == null)
            {
                return null;
            }

            roadmap.IsActive = false;
            roadmap.UpdatedAt = DateTime.Now;
            
            var result = await _trainingRoadmapRepository.UpdateTrainingRoadmapAsync(roadmap);
            var roadmapDto = _mapper.Map<TrainingRoadmapReadDto>(result);
            
            // Get major information from gRPC service
            if (roadmapDto.MajorId.HasValue && roadmapDto.MajorId.Value != Guid.Empty)
            {
                var major = await _grpcClient.GetMajorByIdAsync(roadmapDto.MajorId.Value.ToString());
                if (major.Success && major.Data != null)
                {
                    roadmapDto.MajorData = major.Data;
                }
            }

            // Get batch information for BatchIds
            if (roadmapDto.BatchIds != null && roadmapDto.BatchIds.Any())
            {
                roadmapDto.BatchDatas = await GetBatchDataForIdsAsync(roadmapDto.BatchIds);
            }
            
            return roadmapDto;
        }
        
        public async Task<TrainingRoadmapReadDto> ActivateTrainingRoadmapAsync(Guid id)
        {
            var roadmap = await _trainingRoadmapRepository.GetTrainingRoadmapByIdAsync(id);
            if (roadmap == null)
            {
                return null;
            }

            roadmap.IsActive = true;
            roadmap.UpdatedAt = DateTime.Now;
            
            var result = await _trainingRoadmapRepository.UpdateTrainingRoadmapAsync(roadmap);
            var roadmapDto = _mapper.Map<TrainingRoadmapReadDto>(result);
            
            // Get major information from gRPC service
            if (roadmapDto.MajorId.HasValue && roadmapDto.MajorId.Value != Guid.Empty)
            {
                var major = await _grpcClient.GetMajorByIdAsync(roadmapDto.MajorId.Value.ToString());
                if (major.Success && major.Data != null)
                {
                    roadmapDto.MajorData = major.Data;
                }
            }

            // Get batch information for BatchIds
            if (roadmapDto.BatchIds != null && roadmapDto.BatchIds.Any())
            {
                roadmapDto.BatchDatas = await GetBatchDataForIdsAsync(roadmapDto.BatchIds);
            }
            
            return roadmapDto;
        }        public async Task<TrainingRoadmapListResponse> GetTrainingRoadmapsByPagination(Pagination pagination, TrainingRoadmapFilterParams filterParams, Order? order)
        {
            var result = await _trainingRoadmapRepository.GetAllTrainingRoadmapsPaginationAsync(pagination, filterParams, order);
            var response = _mapper.Map<TrainingRoadmapListResponse>(result);
              // Populate major data and batch data for each roadmap in the list
            foreach (var roadmap in response.Data)
            {
                // Populate course group credits
                await PopulateCoursesGroupCreditsAsync(roadmap);
                
                if (roadmap.MajorId.HasValue && roadmap.MajorId.Value != Guid.Empty)
                {
                    var major = await _grpcClient.GetMajorByIdAsync(roadmap.MajorId.Value.ToString());
                    if (major.Success && major.Data != null)
                    {
                        roadmap.MajorData = major.Data;
                    }
                }
                
                // Get batch information for BatchIds
                if (roadmap.BatchIds != null && roadmap.BatchIds.Any())
                {
                    roadmap.BatchDatas = await GetBatchDataForIdsAsync(roadmap.BatchIds);
                }
            }
            
            return response;
        }

        public async Task<TrainingRoadmapReadDto> UpdateTrainingRoadmapAsync(Guid id, TrainingRoadmapUpdateDto updateDto)
        {
            var existingRoadmap = await _trainingRoadmapRepository.GetTrainingRoadmapByIdAsync(id);
            if (existingRoadmap == null)
            {
                return null;
            }

            // Update the fields
            existingRoadmap.Name = updateDto.Name;
            existingRoadmap.Description = updateDto.Description;
            existingRoadmap.MajorId = updateDto.MajorId ?? existingRoadmap.MajorId;
            existingRoadmap.UpdatedAt = DateTime.Now;
            
            // Update BatchIds if provided
            if (updateDto.BatchIds != null)
            {
                existingRoadmap.BatchIds = updateDto.BatchIds;
            }
              var result = await _trainingRoadmapRepository.UpdateTrainingRoadmapAsync(existingRoadmap);
            var roadmapDto = _mapper.Map<TrainingRoadmapReadDto>(result);
            
            // Populate course group credits
            await PopulateCoursesGroupCreditsAsync(roadmapDto);
            
            // Get major information from gRPC service
            if (roadmapDto.MajorId.HasValue && roadmapDto.MajorId.Value != Guid.Empty)
            {
                var major = await _grpcClient.GetMajorByIdAsync(roadmapDto.MajorId.Value.ToString());
                if (major.Success && major.Data != null)
                {
                    roadmapDto.MajorData = major.Data;
                }
            }

            // Get batch information for BatchIds
            if (roadmapDto.BatchIds != null && roadmapDto.BatchIds.Any())
            {
                roadmapDto.BatchDatas = await GetBatchDataForIdsAsync(roadmapDto.BatchIds);
            }
            
            return roadmapDto;
        }          public async Task<TrainingRoadmapReadDto> GetTrainingRoadmapByIdAsync(Guid id)
        {
            var trainingRoadmap = await _trainingRoadmapRepository.GetTrainingRoadmapByIdAsync(id);
            if (trainingRoadmap == null)
            {
                return null;
            }

            var roadmapDto = _mapper.Map<TrainingRoadmapReadDto>(trainingRoadmap);
            
            // Populate course group credits
            await PopulateCoursesGroupCreditsAsync(roadmapDto);
            
            // Get major information from gRPC service
            if (roadmapDto.MajorId.HasValue && roadmapDto.MajorId.Value != Guid.Empty)
            {
                var major = await _grpcClient.GetMajorByIdAsync(roadmapDto.MajorId.Value.ToString());
                if (major.Success && major.Data != null)
                {
                    roadmapDto.MajorData = major.Data;
                }
            }
            
            // Get batch information for BatchIds
            if (roadmapDto.BatchIds != null && roadmapDto.BatchIds.Any())
            {
                roadmapDto.BatchDatas = await GetBatchDataForIdsAsync(roadmapDto.BatchIds);
            }
            
            return roadmapDto;
        }

        public async Task<TrainingRoadmapReadDto> GetTrainingRoadmapByMajorIdAndBatchIdAsync(Guid majorId, Guid batchId)
        {
            var trainingRoadmap = await _trainingRoadmapRepository.GetTrainingRoadmapByMajorIdAndBatchIdAsync(majorId, batchId);
            if (trainingRoadmap == null)
            {
                return null;
            }

            var roadmapDto = _mapper.Map<TrainingRoadmapReadDto>(trainingRoadmap);
            
            // Populate course group credits
            await PopulateCoursesGroupCreditsAsync(roadmapDto);
            
            // Get major information from gRPC service
            if (roadmapDto.MajorId.HasValue && roadmapDto.MajorId.Value != Guid.Empty)
            {
                var major = await _grpcClient.GetMajorByIdAsync(roadmapDto.MajorId.Value.ToString());
                if (major.Success && major.Data != null)
                {
                    roadmapDto.MajorData = major.Data;
                }
            }
            
            // Get batch information for BatchIds
            if (roadmapDto.BatchIds != null && roadmapDto.BatchIds.Any())
            {
                roadmapDto.BatchDatas = await GetBatchDataForIdsAsync(roadmapDto.BatchIds);
            }
            
            return roadmapDto;
        }
        public async Task<TrainingRoadmapReadDto> AddTrainingRoadmapComponentsAsync(TrainingRoadmapAddComponentsDto componentsDto)
        {
            // Verify that the training roadmap exists
            var existingRoadmap = await _trainingRoadmapRepository.GetTrainingRoadmapByIdAsync(componentsDto.TrainingRoadmapId);
            if (existingRoadmap == null)
            {
                throw new Exception($"Training roadmap with ID {componentsDto.TrainingRoadmapId} not found");
            }
            
            // Map the DTOs to entities
            var coursesGroupSemesters = componentsDto.CoursesGroupSemesters != null 
                ? componentsDto.CoursesGroupSemesters.Select(cgs => new CoursesGroupSemester
                {
                    SemesterNumber = cgs.SemesterNumber,
                    CoursesGroupId = cgs.CoursesGroupId,
                    TrainingRoadmapId = componentsDto.TrainingRoadmapId
                }).ToList()
                : new List<CoursesGroupSemester>();
                
            var trainingRoadmapCourses = componentsDto.TrainingRoadmapCourses != null
                ? componentsDto.TrainingRoadmapCourses.Select(trc => new TrainingRoadmapCourse
                {
                    CourseId = trc.CourseId,
                    SemesterNumber = trc.SemesterNumber,
                    TrainingRoadmapId = componentsDto.TrainingRoadmapId
                }).ToList()
                : new List<TrainingRoadmapCourse>();
            
            // Replace all components (this will clear existing and add new ones)
            var updatedRoadmap = await _trainingRoadmapRepository.AddTrainingRoadmapComponentsAsync(
                componentsDto.TrainingRoadmapId, 
                coursesGroupSemesters, 
                trainingRoadmapCourses);
              // Map the result back to DTO
            var roadmapDto = _mapper.Map<TrainingRoadmapReadDto>(updatedRoadmap);
            
            // Populate course group credits
            await PopulateCoursesGroupCreditsAsync(roadmapDto);
            
            // Get major information from gRPC service
            if (roadmapDto.MajorId.HasValue && roadmapDto.MajorId.Value != Guid.Empty)
            {
                var major = await _grpcClient.GetMajorByIdAsync(roadmapDto.MajorId.Value.ToString());
                if (major.Success && major.Data != null)
                {
                    roadmapDto.MajorData = major.Data;
                }
            }

            // Get batch information for BatchIds
            if (roadmapDto.BatchIds != null && roadmapDto.BatchIds.Any())
            {
                roadmapDto.BatchDatas = await GetBatchDataForIdsAsync(roadmapDto.BatchIds);
            }
            
            return roadmapDto;
        }

        private async Task<List<BatchData>> GetBatchDataForIdsAsync(List<Guid> batchIds)
        {
            if (batchIds == null || !batchIds.Any())
            {                return new List<BatchData>();
            }

            try
            {
                var stringIds = batchIds.Select(id => id.ToString()).ToList();
                return await _grpcBatchClient.GetBatchesByIdsAsync(stringIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching batch data");
                return new List<BatchData>();
            }
        }

        private async Task PopulateCoursesGroupCreditsAsync(TrainingRoadmapReadDto roadmapDto)
        {
            if (roadmapDto.CoursesGroupSemesters == null || !roadmapDto.CoursesGroupSemesters.Any())
                return;

            // Get all unique course group IDs
            var courseGroupIds = roadmapDto.CoursesGroupSemesters
                .Select(cgs => cgs.CoursesGroupId)
                .Distinct()
                .ToList();

            // Fetch course groups and their course details
            foreach (var courseGroupId in courseGroupIds)
            {
                var courseGroup = await _coursesGroupRepository.GetCoursesGroupByIdAsync(courseGroupId);
                if (courseGroup != null && courseGroup.CourseIds.Any())
                {
                    // Get the first course to determine the credit
                    var firstCourse = await _courseRepository.GetCourseByIdAsync(courseGroup.CourseIds.First());
                    if (firstCourse != null)
                    {
                        // Update all CoursesGroupSemesters with this course group ID
                        foreach (var cgs in roadmapDto.CoursesGroupSemesters.Where(x => x.CoursesGroupId == courseGroupId))
                        {
                            cgs.Credit = firstCourse.Credit;
                        }
                    }
                }
            }
        }

        public async Task<bool> DeleteTrainingRoadmapAsync(Guid id)
        {
            // Check if training roadmap exists
            var trainingRoadmap = await _trainingRoadmapRepository.GetTrainingRoadmapByIdAsync(id);
            if (trainingRoadmap == null)
            {
                throw new KeyNotFoundException("Training roadmap not found");
            }

            // Check if training roadmap has been applied to any batches
            if (trainingRoadmap.BatchIds != null && trainingRoadmap.BatchIds.Any())
            {
                throw new InvalidOperationException($"Cannot delete training roadmap '{trainingRoadmap.Name}' because it has been applied to {trainingRoadmap.BatchIds.Count} batch(es). Please remove all batch assignments before deleting the training roadmap.");
            }

            // If no batches are assigned, proceed with deletion
            var result = await _trainingRoadmapRepository.DeleteTrainingRoadmapAsync(id);
            return result;
        }
    }
}