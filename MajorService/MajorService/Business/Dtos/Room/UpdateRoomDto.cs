using System;

namespace MajorService.Business.Dtos.Room
{
    public class UpdateRoomDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? AvailableSeats { get; set; }
    }
}
