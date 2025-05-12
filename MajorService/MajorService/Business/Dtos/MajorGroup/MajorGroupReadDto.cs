using System;

namespace MajorService.Business.Dtos.MajorGroup
{
    public class MajorGroupReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public bool IsActive { get; set; }
    }
}
