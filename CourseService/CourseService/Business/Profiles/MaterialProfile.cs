using AutoMapper;
using CourseService.Business.Dtos.Course;
using CourseService.Business.Dtos.Material;
using CourseService.Entities;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Profiles
{
    public class MaterialProfile : Profile
    {
        public MaterialProfile()
        {
            CreateMap<Material, CourseMaterialCreateDto>();
            CreateMap<Material, CourseMaterialUpdateDto>();
            
            // Map CourseMaterial -> CourseMaterialReadDto
            CreateMap<CourseMaterial, CourseMaterialReadDto>()
                .ForMember(dest => dest.MaterialId, opt => opt.MapFrom(src => src.MaterialId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Material.Name))
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.Material.FileUrl));
        }
    }
}
