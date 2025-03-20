using AutoMapper;
using StudentService.Business.Dtos.Student;
using StudentService.Entities;

namespace StudentService.Business.Profiles
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<Student, StudentReadDto>();
        }
    }
}
