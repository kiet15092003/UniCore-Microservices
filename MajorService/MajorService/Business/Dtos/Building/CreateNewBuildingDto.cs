using System;

namespace MajorService.Business.Dtos.Building
{
    public class CreateNewBuildingDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid LocationId { get; set; }
    }
}
