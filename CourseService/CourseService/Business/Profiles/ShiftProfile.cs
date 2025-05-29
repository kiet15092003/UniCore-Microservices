using AutoMapper;
using CourseService.Business.Dtos.Shift;
using CourseService.Entities;

namespace CourseService.Business.Profiles
{
    public class ShiftProfile : Profile
    {
        public ShiftProfile()
        {
            CreateMap<Shift, ShiftDto>();
        }
    }
}
