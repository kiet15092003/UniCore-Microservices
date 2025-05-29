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
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.Material.FileUrl))
                .ForMember(dest => dest.MaterialTypeId, opt => opt.MapFrom(src => src.Material.MaterialTypeId))
                .ForMember(dest => dest.MaterialTypeName, opt => opt.MapFrom(src => src.Material.MaterialType != null ? src.Material.MaterialType.Name : null));

            // Map MaterialType
            CreateMap<PaginationResult<Material>, MaterialListResponse>();
            CreateMap<Material, MaterialReadDto>();
            CreateMap<Material, MaterialReadDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.FileUrl))
                .ForMember(dest => dest.MaterialTypeId, opt => opt.MapFrom(src => src.MaterialType.Id));
            CreateMap<MaterialType, MaterialTypeReadDto>();
            CreateMap<MaterialTypeCreateDto, MaterialType>();
            CreateMap<MaterialTypeUpdateDto, MaterialType>();
        }
    }
}
