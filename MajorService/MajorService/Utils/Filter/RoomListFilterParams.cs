using System;

namespace MajorService.Utils.Filter
{
    public class RoomListFilterParams
    {
        public string? Name { get; set; }
        public Guid? FloorId { get; set; }
        public Guid? LocationId { get; set; }
        public Guid? BuildingId { get; set; }
        public bool? IsActive { get; set; }
        public int? AvailableSeats { get; set; }    
    }
}
