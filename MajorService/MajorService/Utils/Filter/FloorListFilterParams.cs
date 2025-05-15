using System;

namespace MajorService.Utils.Filter
{
    public class FloorListFilterParams
    {
        public string? Name { get; set; }
        public Guid? BuildingId { get; set; }
        public bool? IsActive { get; set; }
    }
}
