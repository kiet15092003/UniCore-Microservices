namespace EnrollmentService.Utils.Exceptions
{
    public class EnrollmentCapacityExceededException : Exception
    {
        public Guid AcademicClassId { get; }
        public int CurrentCount { get; }
        public int Capacity { get; }

        public EnrollmentCapacityExceededException(Guid academicClassId, int currentCount, int capacity)
            : base($"Academic class {academicClassId} has reached its capacity. Current: {currentCount}, Capacity: {capacity}")
        {
            AcademicClassId = academicClassId;
            CurrentCount = currentCount;
            Capacity = capacity;
        }

        public EnrollmentCapacityExceededException(Guid academicClassId, int currentCount, int capacity, Exception innerException)
            : base($"Academic class {academicClassId} has reached its capacity. Current: {currentCount}, Capacity: {capacity}", innerException)
        {
            AcademicClassId = academicClassId;
            CurrentCount = currentCount;
            Capacity = capacity;
        }
    }

    public class EnrollmentTransactionException : Exception
    {
        public List<Guid> FailedEnrollments { get; }

        public EnrollmentTransactionException(string message, List<Guid> failedEnrollments) : base(message)
        {
            FailedEnrollments = failedEnrollments;
        }

        public EnrollmentTransactionException(string message, List<Guid> failedEnrollments, Exception innerException) : base(message, innerException)
        {
            FailedEnrollments = failedEnrollments;
        }
    }
}
