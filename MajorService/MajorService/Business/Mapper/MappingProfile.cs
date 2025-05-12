using AutoMapper;
using MajorService.Business.Dtos.Department;
using MajorService.Business.Dtos.MajorGroup;
using MajorService.Entities;

namespace MajorService.Business.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Department mappings
            CreateMap<Department, DepartmentReadDto>();

            // MajorGroup mappings
            CreateMap<MajorGroup, MajorGroupReadDto>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty));
        }
    }
}
