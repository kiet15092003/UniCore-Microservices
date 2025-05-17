using System;
using System.Collections.Generic;
using MajorService.Business.Dtos.Building;
using MajorService.Business.Dtos.Room;

namespace MajorService.Business.Dtos.Floor
{
    public class FloorReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public BuildingReadDto Building { get; set; }
        public int TotalRoom { get; set; }
    }
}
