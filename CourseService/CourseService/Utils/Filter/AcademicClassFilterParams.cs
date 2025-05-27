using System;
using System.Collections.Generic;

namespace CourseService.Utils.Filter
{
    public class AcademicClassFilterParams
    {
        public string? Name { get; set; }
        public int? GroupNumber { get; set; }
        public int? MinCapacity { get; set; }
        public int? MaxCapacity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsRegistrable { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? SemesterId { get; set; }
        public Guid? RoomId { get; set; }
        public Guid? ShiftId { get; set; }
        public List<Guid>? ScheduleInDayIds { get; set; }
    }
}
