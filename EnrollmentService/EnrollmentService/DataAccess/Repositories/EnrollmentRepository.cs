using EnrollmentService.Entities;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using CourseService.DataAccess;
using EnrollmentService.CommunicationTypes.Grpc.GrpcClient;

namespace EnrollmentService.DataAccess.Repositories
{    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly AppDbContext _context;
        private readonly GrpcAcademicClassClientService _grpcAcademicClassClient;
        private readonly GrpcStudentClientService _grpcStudentClient;

        public EnrollmentRepository(AppDbContext context, GrpcAcademicClassClientService grpcAcademicClassClient, GrpcStudentClientService grpcStudentClient)
        {
            _context = context;
            _grpcAcademicClassClient = grpcAcademicClassClient;
            _grpcStudentClient = grpcStudentClient;
        }

        public async Task<Enrollment?> GetEnrollmentByIdAsync(Guid id)
        {
            return await _context.Enrollments
                .Include(e => e.StudentResults)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<PaginationResult<Enrollment>> GetAllEnrollmentsPaginationAsync(
            Pagination pagination,
            EnrollmentListFilterParams filterParams,
            Order? order)
        {
            var query = _context.Enrollments.AsQueryable();            
            // Apply filters
            if (filterParams != null)
            {
                if (!string.IsNullOrEmpty(filterParams.StudentCode))
                {
                    // Get student ID from student code
                    var studentId = await GetStudentIdByCodeAsync(filterParams.StudentCode);
                    if (studentId.HasValue)
                    {
                        query = query.Where(e => e.StudentId == studentId.Value);
                    }
                    else
                    {
                        // If student not found, return empty result
                        query = query.Where(e => false);
                    }
                }

                if (filterParams.AcademicClassId.HasValue)
                {
                    query = query.Where(e => e.AcademicClassId == filterParams.AcademicClassId.Value);
                }

                if (filterParams.Status.HasValue)
                {
                    query = query.Where(e => e.Status == filterParams.Status.Value);
                }                // Filter by semester and course if specified
                if (filterParams.SemesterId.HasValue || filterParams.CourseId.HasValue)
                {
                    // Get all academic class IDs that match the semester and/or course criteria
                    var matchingAcademicClassIds = await GetMatchingAcademicClassIdsAsync(
                        filterParams.SemesterId, 
                        filterParams.CourseId);
                    
                    if (matchingAcademicClassIds.Any())
                    {
                        query = query.Where(e => matchingAcademicClassIds.Contains(e.AcademicClassId));
                    }
                    else
                    {
                        // If no matching academic classes found, return empty result
                        query = query.Where(e => false);
                    }
                }

                // Filter by date range
                if (filterParams.FromDate.HasValue)
                {
                    query = query.Where(e => e.CreatedAt >= filterParams.FromDate.Value);
                }

                if (filterParams.ToDate.HasValue)
                {
                    // Add time to include the entire day
                    var toDate = filterParams.ToDate.Value.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(e => e.CreatedAt <= toDate);
                }
            }            // Apply ordering
            if (order != null)
            {
                switch (order.OrderBy.ToLower())
                {
                    case "enrolleddate":
                    case "createdat":
                        query = order.IsDesc 
                            ? query.OrderByDescending(e => e.CreatedAt) 
                            : query.OrderBy(e => e.CreatedAt);
                        break;
                    case "status":
                        query = order.IsDesc 
                            ? query.OrderByDescending(e => e.Status) 
                            : query.OrderBy(e => e.Status);
                        break;
                    case "studentname":
                    case "studentcode":
                    case "coursename":
                    case "classname":
                    case "semester":
                        // For fields that require external data, we'll sort in memory after fetching
                        // For now, apply default ordering and sort later
                        query = query.OrderBy(e => e.CreatedAt);
                        break;
                    default:
                        query = order.IsDesc 
                            ? query.OrderByDescending(e => e.CreatedAt)
                            : query.OrderBy(e => e.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(e => e.CreatedAt);
            }            // Get total count
            var totalCount = await query.CountAsync();

            // Check if we need complex sorting that requires external data
            bool needsComplexSorting = order != null && IsComplexSortField(order.OrderBy);
            
            List<Enrollment> items;
            
            if (needsComplexSorting)
            {
                // For complex sorting, get all filtered items then sort in memory
                var allItems = await query
                    .Include(e => e.StudentResults)
                    .ToListAsync();
                
                // Apply complex sorting
                var sortedItems = await ApplyComplexSortingAsync(allItems, order);
                
                // Apply pagination to sorted results
                items = sortedItems
                    .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                    .Take(pagination.ItemsPerpage)
                    .ToList();
            }
            else
            {
                // Apply pagination for simple sorting
                items = await query
                    .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                    .Take(pagination.ItemsPerpage)
                    .Include(e => e.StudentResults)
                    .ToListAsync();
            }

            // Create pagination result
            return new PaginationResult<Enrollment>
            {
                Data = items,
                Total = totalCount,
                PageSize = pagination.ItemsPerpage,
                PageIndex = pagination.PageNumber - 1
            };
        }

        public async Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment)
        {
            enrollment.Id = Guid.NewGuid();
            enrollment.CreatedAt = DateTime.UtcNow;
            enrollment.UpdatedAt = DateTime.UtcNow;
            
            var newEnrollment = await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();
            return newEnrollment.Entity;
        }
        public async Task<List<Enrollment>> CreateMultipleEnrollmentsAsync(List<Enrollment> enrollments)
        {
            foreach (var enrollment in enrollments)
            {
                enrollment.Id = Guid.NewGuid();
                enrollment.CreatedAt = DateTime.UtcNow;
                enrollment.UpdatedAt = DateTime.UtcNow;
            }

            await _context.Enrollments.AddRangeAsync(enrollments);
            await _context.SaveChangesAsync();
            return enrollments;
        }
        public async Task<List<Enrollment>> CreateMultipleEnrollmentsWithTransactionAsync(List<Enrollment> enrollments)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var enrollment in enrollments)
                {
                    enrollment.Id = Guid.NewGuid();
                    enrollment.CreatedAt = DateTime.UtcNow;
                    enrollment.UpdatedAt = DateTime.UtcNow;
                }

                await _context.Enrollments.AddRangeAsync(enrollments);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return enrollments;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Enrollment>> CreateMultipleEnrollmentsWithoutTransactionAsync(List<Enrollment> enrollments)
        {
            foreach (var enrollment in enrollments)
            {
                enrollment.Id = Guid.NewGuid();
                enrollment.CreatedAt = DateTime.UtcNow;
                enrollment.UpdatedAt = DateTime.UtcNow;
            }

            await _context.Enrollments.AddRangeAsync(enrollments);
            await _context.SaveChangesAsync();
            return enrollments;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<bool> ExistsAsync(Guid studentId, Guid academicClassId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.AcademicClassId == academicClassId);
        }

        public async Task<int> GetEnrollmentCountByAcademicClassIdAsync(Guid academicClassId)
        {
            return await _context.Enrollments
                .Where(e => e.AcademicClassId == academicClassId)
                .CountAsync();
        }

        public async Task<int> GetEnrollmentCountByAcademicClassIdWithLockAsync(Guid academicClassId)
        {
            // Use SELECT FOR UPDATE equivalent in SQL Server (WITH (UPDLOCK, HOLDLOCK))
            var count = await _context.Database
               .SqlQuery<int>($"SELECT COUNT(*) AS Value FROM Enrollments WITH (UPDLOCK, HOLDLOCK) WHERE AcademicClassId = {academicClassId}")
               .FirstAsync();

            return count;
        }
        public async Task<List<Enrollment>> GetEnrollmentsByStudentIdAsync(Guid studentId, Guid? semesterId = null)
        {
            var query = _context.Enrollments
                .Include(e => e.StudentResults)
                .Where(e => e.StudentId == studentId);

            // Note: Since we're in a microservices architecture and AcademicClass is in a different service,
            // we can't filter by semesterId at the database level. This filtering will be done at the service level.
            return await query.ToListAsync();
        }

        public async Task<bool> DeleteEnrollmentAsync(Guid id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.StudentResults)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
            {
                return false;
            }

            // Remove related student results first
            if (enrollment.StudentResults != null && enrollment.StudentResults.Any())
            {
                _context.StudentResults.RemoveRange(enrollment.StudentResults);
            }

            // Remove the enrollment
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> UpdateEnrollmentStatusByClassIdsAsync(List<Guid> classIds, int fromStatus, int toStatus)
        {
            var enrollments = await _context.Enrollments
                .Where(e => classIds.Contains(e.AcademicClassId) && e.Status == fromStatus)
                .ToListAsync();

            if (enrollments.Any())
            {
                foreach (var enrollment in enrollments)
                {
                    enrollment.Status = toStatus;
                }

                await _context.SaveChangesAsync();
            }

            return enrollments.Count;
        }

        private async Task<List<Guid>> GetMatchingAcademicClassIdsAsync(Guid? semesterId, Guid? courseId)
        {
            var matchingIds = new List<Guid>();

            // Get all unique academic class IDs from enrollments
            var academicClassIds = await _context.Enrollments
                .Select(e => e.AcademicClassId)
                .Distinct()
                .ToListAsync();

            // For each academic class ID, check if it matches the criteria
            foreach (var academicClassId in academicClassIds)
            {
                try
                {
                    var academicClassResponse = await _grpcAcademicClassClient.GetAcademicClassById(academicClassId.ToString());
                    
                    if (academicClassResponse?.Success == true && academicClassResponse.Data != null)
                    {
                        var academicClass = academicClassResponse.Data;
                        bool matches = true;

                        // Check semester filter
                        if (semesterId.HasValue)
                        {
                            if (!Guid.TryParse(academicClass.SemesterId, out var academicClassSemesterId) || 
                                academicClassSemesterId != semesterId.Value)
                            {
                                matches = false;
                            }
                        }

                        // Check course filter
                        if (courseId.HasValue && matches)
                        {
                            if (!Guid.TryParse(academicClass.CourseId, out var academicClassCourseId) || 
                                academicClassCourseId != courseId.Value)
                            {
                                matches = false;
                            }
                        }

                        if (matches)
                        {
                            matchingIds.Add(academicClassId);
                        }
                    }
                }
                catch (Exception)
                {
                    // If gRPC call fails, skip this academic class
                    // In a production environment, you might want to log this error
                    continue;
                }
            }

            return matchingIds;
        }

        private async Task<Guid?> GetStudentIdByCodeAsync(string studentCode)
        {
            try
            {
                // Get all unique student IDs from enrollments
                var studentIds = await _context.Enrollments
                    .Select(e => e.StudentId)
                    .Distinct()
                    .ToListAsync();

                // For each student ID, check if it matches the student code
                foreach (var studentId in studentIds)
                {
                    try
                    {
                        var studentResponse = await _grpcStudentClient.GetStudentById(studentId.ToString());
                        
                        if (studentResponse?.Success == true && 
                            studentResponse.Data != null && 
                            studentResponse.Data.StudentCode.Equals(studentCode, StringComparison.OrdinalIgnoreCase))
                        {
                            return studentId;
                        }
                    }
                    catch (Exception)
                    {
                        // If gRPC call fails, skip this student
                        continue;
                    }
                }

                return null;
            }
            catch (Exception)
            {
                // If any error occurs, return null
                return null;
            }
        }

        private bool IsComplexSortField(string orderBy)
        {
            var complexFields = new[] { "studentname", "studentcode", "coursename", "classname", "semester" };
            return complexFields.Contains(orderBy.ToLower());
        }

        private async Task<List<Enrollment>> ApplyComplexSortingAsync(List<Enrollment> enrollments, Order order)
        {
            // Create a list to hold enrollment data with external information
            var enrollmentDataList = new List<EnrollmentSortData>();

            foreach (var enrollment in enrollments)
            {
                var sortData = new EnrollmentSortData 
                { 
                    Enrollment = enrollment,
                    StudentName = "",
                    StudentCode = "",
                    CourseName = "",
                    ClassName = "",
                    SemesterInfo = ""
                };

                try
                {
                    // Get student data
                    var studentResponse = await _grpcStudentClient.GetStudentById(enrollment.StudentId.ToString());
                    if (studentResponse?.Success == true && studentResponse.Data != null)
                    {
                        sortData.StudentName = $"{studentResponse.Data.User.FirstName} {studentResponse.Data.User.LastName}".Trim();
                        sortData.StudentCode = studentResponse.Data.StudentCode ?? "";
                    }

                    // Get academic class data
                    var academicClassResponse = await _grpcAcademicClassClient.GetAcademicClassById(enrollment.AcademicClassId.ToString());
                    if (academicClassResponse?.Success == true && academicClassResponse.Data != null)
                    {
                        var academicClass = academicClassResponse.Data;
                        sortData.ClassName = academicClass.Name ?? "";
                        
                        if (academicClass.Course != null)
                        {
                            sortData.CourseName = academicClass.Course.Name ?? "";
                        }
                        
                        if (academicClass.Semester != null)
                        {
                            sortData.SemesterInfo = $"Semester {academicClass.Semester.SemesterNumber} - {academicClass.Semester.Year}";
                        }
                    }
                }
                catch (Exception)
                {
                    // If external calls fail, use empty strings for sorting
                }

                enrollmentDataList.Add(sortData);
            }

            // Apply sorting based on the order field
            IOrderedEnumerable<EnrollmentSortData> sortedData;
            
            switch (order.OrderBy.ToLower())
            {
                case "studentname":
                    sortedData = order.IsDesc 
                        ? enrollmentDataList.OrderByDescending(x => x.StudentName)
                        : enrollmentDataList.OrderBy(x => x.StudentName);
                    break;
                case "studentcode":
                    sortedData = order.IsDesc 
                        ? enrollmentDataList.OrderByDescending(x => x.StudentCode)
                        : enrollmentDataList.OrderBy(x => x.StudentCode);
                    break;
                case "coursename":
                    sortedData = order.IsDesc 
                        ? enrollmentDataList.OrderByDescending(x => x.CourseName)
                        : enrollmentDataList.OrderBy(x => x.CourseName);
                    break;
                case "classname":
                    sortedData = order.IsDesc 
                        ? enrollmentDataList.OrderByDescending(x => x.ClassName)
                        : enrollmentDataList.OrderBy(x => x.ClassName);
                    break;
                case "semester":
                    sortedData = order.IsDesc 
                        ? enrollmentDataList.OrderByDescending(x => x.SemesterInfo)
                        : enrollmentDataList.OrderBy(x => x.SemesterInfo);
                    break;
                default:
                    sortedData = order.IsDesc 
                        ? enrollmentDataList.OrderByDescending(x => x.Enrollment.CreatedAt)
                        : enrollmentDataList.OrderBy(x => x.Enrollment.CreatedAt);
                    break;
            }

            return sortedData.Select(x => x.Enrollment).ToList();
        }

        private class EnrollmentSortData
        {
            public Enrollment Enrollment { get; set; }
            public string StudentName { get; set; }
            public string StudentCode { get; set; }
            public string CourseName { get; set; }
            public string ClassName { get; set; }
            public string SemesterInfo { get; set; }
        }
    }
}
