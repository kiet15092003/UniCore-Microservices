using AutoMapper;
using MajorService.Business.Dtos.Building;
using MajorService.Business.Dtos.Department;
using MajorService.Business.Dtos.Floor;
using MajorService.Business.Dtos.Location;
using MajorService.Business.Dtos.Major;
using MajorService.Business.Dtos.MajorGroup;
using MajorService.Business.Dtos.Room;
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
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department));

            // Major mappings
            CreateMap<Major, MajorReadDto>()
                .ForMember(dest => dest.MajorGroup, opt => opt.MapFrom(src => src.MajorGroup))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            // Location mappings
            CreateMap<CreateNewLocationDto, Location>();
            CreateMap<Location, LocationReadDto>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            // Building mappings
            CreateMap<CreateNewBuildingDto, Building>();
            CreateMap<Building, BuildingReadDto>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            // Floor mappings
            CreateMap<CreateNewFloorDto, Floor>();
            CreateMap<Floor, FloorReadDto>()
                .ForMember(dest => dest.Building, opt => opt.MapFrom(src => src.Building))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            // Room mappings
            CreateMap<CreateNewRoomDto, Room>();
            CreateMap<Room, RoomReadDto>()
                .ForMember(dest => dest.Floor, opt => opt.MapFrom(src => src.Floor))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
