using AutoMapper;
using CourseService.Business.Dtos.Student;
using CourseService.Entities;

namespace CourseService.Business.Profiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Student, StudentReadDto>();
        }
    }
}
