using MajorService.Business.Dtos.Floor;
using System;

namespace MajorService.Business.Dtos.Room
{
    public class RoomReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int AvailableSeats { get; set; }
        public FloorReadDto Floor { get; set; }
    }
}
