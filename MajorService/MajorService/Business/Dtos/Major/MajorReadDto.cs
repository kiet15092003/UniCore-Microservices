using System;
using MajorService.Business.Dtos.MajorGroup;

namespace MajorService.Business.Dtos.Major
{
    public class MajorReadDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public double CostPerCredit { get; set; }
        public MajorGroupReadDto? MajorGroup { get; set; }
        public bool IsActive { get; set; }
    }
}
