using System;

namespace MajorService.Entities
{
    public class Room : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public Guid FloorId { get; set; }
        public Floor? Floor { get; set; }
    }
}
