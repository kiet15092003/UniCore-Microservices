using AutoMapper;
using CourseService.Business.Dtos.Course;
using CourseService.Entities;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Profiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<CourseCreateDto, Course>()
                .ForMember(dest => dest.MajorIds, opt => opt.MapFrom(src => src.MajorIds));


            // Map Course -> CourseReadDto
            CreateMap<Course, CourseReadDto>()
                .ForMember(dest => dest.CourseCertificates, opt => opt.MapFrom(src => src.CourseCertificates))
                .ForMember(dest => dest.CourseMaterials, opt => opt.MapFrom(src => src.CourseMaterials))
                .ForMember(dest => dest.MajorIds, opt => opt.MapFrom(src => src.MajorIds));

            // Map CourseCertificate -> CourseCertificateReadDto
            CreateMap<CourseCertificate, CourseCertificateReadDto>()
                .ForMember(dest => dest.CertificateId, opt => opt.MapFrom(src => src.CertificateId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Certificate.Name))
                .ForMember(dest => dest.RequiredScore, opt => opt.MapFrom(src => src.Certificate.RequiredScore));

            // Map CourseMaterial -> CourseMaterialReadDto
            CreateMap<CourseMaterial, CourseMaterialReadDto>()
                .ForMember(dest => dest.MaterialId, opt => opt.MapFrom(src => src.MaterialId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Material.Name))
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.Material.FileUrl));

            CreateMap<PaginationResult<Course>, CourseListResponse>();

            CreateMap<CourseUpdateDto, Course>()
                .ForMember(dest => dest.MajorIds, opt => opt.MapFrom(src => src.MajorIds));
        }
    }
}
