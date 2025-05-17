using System;

namespace MajorService.Business.Dtos.Floor
{
    public class CreateNewFloorDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid BuildingId { get; set; }
    }
}
