using System;

namespace MajorService.Business.Dtos.Room
{
    public class CreateNewRoomDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid FloorId { get; set; }
        public int AvailableSeats { get; set; }
    }
}
