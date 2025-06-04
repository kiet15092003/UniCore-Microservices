using AutoMapper;
using CourseService.Business.Dtos.TrainingRoadmap;
using CourseService.Entities;
using CourseService.Utils.Pagination;
using System.Linq;

namespace CourseService.Business.Profiles
{
    public class TrainingRoadmapProfile : Profile
    {
        public TrainingRoadmapProfile()
        {            // TrainingRoadmap -> TrainingRoadmapReadDto
            CreateMap<TrainingRoadmap, TrainingRoadmapReadDto>()
                .ForMember(dest => dest.CoursesGroupSemesters, opt => opt.MapFrom(src => src.CoursesGroupSemesters))
                .ForMember(dest => dest.TrainingRoadmapCourses, opt => opt.MapFrom(src => src.TrainingRoadmapCourses));
                  // Explicitly handle mappings to avoid circular references
            CreateMap<CoursesGroupSemester, CoursesGroupSemesterReadDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SemesterNumber, opt => opt.MapFrom(src => src.SemesterNumber))
                .ForMember(dest => dest.CoursesGroupId, opt => opt.MapFrom(src => src.CoursesGroupId))
                .ForMember(dest => dest.CoursesGroupName, opt => opt.MapFrom(src => src.CoursesGroup != null ? src.CoursesGroup.GroupName : null))
                .ForMember(dest => dest.Credit, opt => opt.MapFrom(src => src.CoursesGroup != null ? src.CoursesGroup.Credit : 0))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
                  CreateMap<TrainingRoadmapCourse, TrainingRoadmapCourseReadDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.SemesterNumber, opt => opt.MapFrom(src => src.SemesterNumber))
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

            // TrainingRoadmapCreateDto -> TrainingRoadmap
            CreateMap<TrainingRoadmapCreateDto, TrainingRoadmap>();
            
            // TrainingRoadmapUpdateDto -> TrainingRoadmap
            CreateMap<TrainingRoadmapUpdateDto, TrainingRoadmap>();
            
            // PaginationResult<TrainingRoadmap> -> TrainingRoadmapListResponse
            CreateMap<PaginationResult<TrainingRoadmap>, TrainingRoadmapListResponse>()
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
                .ForMember(dest => dest.PageIndex, opt => opt.MapFrom(src => src.PageIndex));
        }
    }
}