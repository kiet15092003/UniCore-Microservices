using System;
using System.Collections.Generic;

namespace MajorService.Entities
{
    public class Floor : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public Guid BuildingId { get; set; }
        public Building? Building { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
