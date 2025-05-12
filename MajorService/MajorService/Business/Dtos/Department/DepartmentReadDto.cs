using System;

namespace MajorService.Business.Dtos.Department
{
    public class DepartmentReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
    }
}
