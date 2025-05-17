using System;
using System.Collections.Generic;

namespace MajorService.Entities
{
    public class Building : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public Guid LocationId { get; set; }
        public Location? Location { get; set; }
        public ICollection<Floor> Floors { get; set; } = new List<Floor>();
    }
}
