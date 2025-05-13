using System;
using MajorService.Business.Dtos.Department;

namespace MajorService.Business.Dtos.MajorGroup
{
    public class MajorGroupReadDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public DepartmentReadDto? Department { get; set; }
        public bool IsActive { get; set; }
    }
}
