namespace CourseService.Business.Dtos.TrainingRoadmap
{
    public class TrainingRoadmapAddComponentsDto
    {
        /// <summary>
        /// The ID of the TrainingRoadmap to update
        /// </summary>
        public Guid TrainingRoadmapId { get; set; }
        
        /// <summary>
        /// The complete list of CoursesGroupSemesters to set for the TrainingRoadmap.
        /// All existing CoursesGroupSemesters will be removed and replaced with these.
        /// </summary>
        public List<CoursesGroupSemesterDto>? CoursesGroupSemesters { get; set; }
        
        /// <summary>
        /// The complete list of TrainingRoadmapCourses to set for the TrainingRoadmap.
        /// All existing TrainingRoadmapCourses will be removed and replaced with these.
        /// </summary>
        public List<TrainingRoadmapCourseDto>? TrainingRoadmapCourses { get; set; }
    }

    public class CoursesGroupSemesterDto
    {
        public int SemesterNumber { get; set; }
        public Guid CoursesGroupId { get; set; }
    }

    public class TrainingRoadmapCourseDto
    {
        public Guid CourseId { get; set; }
        public int SemesterNumber { get; set; }
    }
}
