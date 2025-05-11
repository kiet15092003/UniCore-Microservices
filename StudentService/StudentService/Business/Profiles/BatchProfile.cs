using AutoMapper;
using StudentService.Business.Dtos.Batch;
using StudentService.Entities;

namespace StudentService.Business.Profiles
{
    public class BatchProfile : Profile
    {
        public BatchProfile()
        {
            CreateMap<Batch, BatchReadDto>();
            CreateMap<BatchCreateDto, Batch>();
            CreateMap<BatchUpdateDto, Batch>();
        }
    }
} 