using System;

namespace MajorService.Business.Dtos.Department
{    public class DepartmentReadDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public bool IsActive { get; set; }
    }
}
