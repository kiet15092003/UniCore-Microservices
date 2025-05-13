using System;

namespace MajorService.Utils.Filter
{
    public class MajorGroupListFilterParams
    {
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
