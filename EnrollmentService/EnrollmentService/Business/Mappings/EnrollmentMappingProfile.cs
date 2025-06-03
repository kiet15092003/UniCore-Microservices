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
            
            // PaginationResult mapping
            CreateMap<PaginationResult<Enrollment>, PaginationResult<EnrollmentReadDto>>();
            CreateMap<PaginationResult<EnrollmentReadDto>, EnrollmentListResponse>()
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Items));
        }
    }
}
