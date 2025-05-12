using AutoMapper;
using CourseService.Business.Dtos.TrainingRoadmap;
using CourseService.CommunicationTypes.Grpc.GrpcClient;
using CourseService.DataAccess.Repositories;
using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Services
{
    public class TrainingRoadmapSvc : ITrainingRoadmapService
    {
        private readonly ITrainingRoadmapRepository _trainingRoadmapRepository;
        private readonly IMapper _mapper;
        private readonly GrpcMajorClientService _grpcClient;

        public TrainingRoadmapSvc(
            ITrainingRoadmapRepository trainingRoadmapRepository,
            IMapper mapper,
            GrpcMajorClientService grpcClient)
        {
            _trainingRoadmapRepository = trainingRoadmapRepository;
            _mapper = mapper;
            _grpcClient = grpcClient;
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
            
            // Get major information from gRPC service
            if (roadmapDto.MajorId.HasValue && roadmapDto.MajorId.Value != Guid.Empty)
            {
                var major = await _grpcClient.GetMajorByIdAsync(roadmapDto.MajorId.Value.ToString());
                if (major.Success && major.Data != null)
                {
                    roadmapDto.MajorData = major.Data;
                }
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
            
            return roadmapDto;
        }

        public async Task<TrainingRoadmapListResponse> GetTrainingRoadmapsByPagination(Pagination pagination, TrainingRoadmapFilterParams filterParams, Order? order)
        {
            var result = await _trainingRoadmapRepository.GetAllTrainingRoadmapsPaginationAsync(pagination, filterParams, order);
            var response = _mapper.Map<TrainingRoadmapListResponse>(result);
            
            // Populate major data for each roadmap in the list
            foreach (var roadmap in response.Data)
            {
                if (roadmap.MajorId.HasValue && roadmap.MajorId.Value != Guid.Empty)
                {
                    var major = await _grpcClient.GetMajorByIdAsync(roadmap.MajorId.Value.ToString());
                    if (major.Success && major.Data != null)
                    {
                        roadmap.MajorData = major.Data;
                    }
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
            existingRoadmap.StartYear = updateDto.StartYear;
            existingRoadmap.UpdatedAt = DateTime.Now;
            
            var result = await _trainingRoadmapRepository.UpdateTrainingRoadmapAsync(existingRoadmap);
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
            
            return roadmapDto;
        }

        public async Task<TrainingRoadmapReadDto> GetTrainingRoadmapByIdAsync(Guid id)
        {
            var trainingRoadmap = await _trainingRoadmapRepository.GetTrainingRoadmapByIdAsync(id);
            if (trainingRoadmap == null)
            {
                return null;
            }

            var roadmapDto = _mapper.Map<TrainingRoadmapReadDto>(trainingRoadmap);
            
            // Get major information from gRPC service
            if (roadmapDto.MajorId.HasValue && roadmapDto.MajorId.Value != Guid.Empty)
            {
                var major = await _grpcClient.GetMajorByIdAsync(roadmapDto.MajorId.Value.ToString());
                if (major.Success && major.Data != null)
                {
                    roadmapDto.MajorData = major.Data;
                }
            }
            
            return roadmapDto;
        }        public async Task<TrainingRoadmapReadDto> AddTrainingRoadmapComponentsAsync(TrainingRoadmapAddComponentsDto componentsDto)
        {            // Verify that the training roadmap exists
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
            
            // Get major information from gRPC service
            if (roadmapDto.MajorId.HasValue && roadmapDto.MajorId.Value != Guid.Empty)
            {
                var major = await _grpcClient.GetMajorByIdAsync(roadmapDto.MajorId.Value.ToString());
                if (major.Success && major.Data != null)
                {
                    roadmapDto.MajorData = major.Data;
                }
            }
            
            return roadmapDto;
        }
    }
}