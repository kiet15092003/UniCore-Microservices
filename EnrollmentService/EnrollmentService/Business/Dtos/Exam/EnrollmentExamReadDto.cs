using System;
using System.Collections.Generic;
using EnrollmentService.Business.Dtos.Enrollment;

namespace EnrollmentService.Business.Dtos.Exam
{
    public class EnrollmentExamReadDto
    {
        public Guid Id { get; set; }
        public Guid ExamId { get; set; }
        public Guid EnrollmentId { get; set; }
        public Guid StudentId { get; set; }
        public GrpcStudentData? Student { get; set; }
    }
}
