using AutoMapper;
using UserService.Business.Dtos.Batch;
using UserService.Entities;

namespace UserService.Business.Profiles
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