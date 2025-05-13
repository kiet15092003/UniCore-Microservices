using System;

namespace MajorService.Utils.Filter
{
    public class DepartmentListFilterParams
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public bool? IsActive { get; set; }
    }
}
