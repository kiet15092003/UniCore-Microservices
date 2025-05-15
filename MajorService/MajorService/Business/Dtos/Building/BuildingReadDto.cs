using System;
using System.Collections.Generic;
using MajorService.Business.Dtos.Floor;
using MajorService.Business.Dtos.Location;

namespace MajorService.Business.Dtos.Building
{
    public class BuildingReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public LocationReadDto Location { get; set; }
    }
}
