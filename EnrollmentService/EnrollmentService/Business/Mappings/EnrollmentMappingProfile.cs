using AutoMapper;
using EnrollmentService.Business.Dtos.Enrollment;
using EnrollmentService.Entities;
using EnrollmentService.Utils.Pagination;

namespace EnrollmentService.Business.Mappings
{
    public class EnrollmentMappingProfile : Profile
    {
        public EnrollmentMappingProfile()
        {
            // Enrollment mapping
            CreateMap<Enrollment, EnrollmentReadDto>();
            CreateMap<MultipleEnrollmentCreateDto, Enrollment>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 1))
                .ForMember(dest => dest.StudentResults, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
            
            // PaginationResult mapping
            CreateMap<PaginationResult<Enrollment>, PaginationResult<EnrollmentReadDto>>();
            CreateMap<PaginationResult<EnrollmentReadDto>, EnrollmentListResponse>()
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data));
        }
    }
}
