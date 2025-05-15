using System;

namespace MajorService.Business.Dtos.Building
{
    public class BuildingListFilterParams
    {
        public string? Name { get; set; }
        public Guid? LocationId { get; set; }
    }
}
