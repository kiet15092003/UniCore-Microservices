using EnrollmentService.Entities;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;
using Microsoft.EntityFrameworkCore.Storage;

namespace EnrollmentService.DataAccess.Repositories
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment?> GetEnrollmentByIdAsync(Guid id);
        Task<PaginationResult<Enrollment>> GetAllEnrollmentsPaginationAsync(
                Pagination pagination,
                EnrollmentListFilterParams enrollmentListFilterParams,
                Order? order);
        Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment);        Task<List<Enrollment>> CreateMultipleEnrollmentsAsync(List<Enrollment> enrollments);
        Task<List<Enrollment>> CreateMultipleEnrollmentsWithTransactionAsync(List<Enrollment> enrollments);
        Task<List<Enrollment>> CreateMultipleEnrollmentsWithoutTransactionAsync(List<Enrollment> enrollments);
        Task<bool> ExistsAsync(Guid studentId, Guid academicClassId);
        Task<int> GetEnrollmentCountByAcademicClassIdAsync(Guid academicClassId);
        Task<List<Enrollment>> GetEnrollmentsByAcademicClassIdAsync(Guid academicClassId);
        Task<int> GetEnrollmentCountByAcademicClassIdWithLockAsync(Guid academicClassId);        Task<List<Enrollment>> GetEnrollmentsByStudentIdAsync(Guid studentId, Guid? semesterId = null);
        Task<List<Enrollment>> GetEnrollmentsByIdsAsync(List<Guid> enrollmentIds);
        Task<bool> DeleteEnrollmentAsync(Guid id);
        Task<int> UpdateEnrollmentStatusByClassIdsAsync(List<Guid> classIds, int fromStatus, int toStatus);        Task<int> ApproveEnrollmentsByAcademicClassIdAsync(Guid classId);
        Task<int> RejectEnrollmentsByAcademicClassIdAsync(Guid classId);
        Task<int> MoveEnrollmentsToNewClassAsync(List<Guid> enrollmentIds, Guid toClassId);
        Task<IDbContextTransaction> BeginTransactionAsync();        Task<int?> GetFirstEnrollmentStatusByAcademicClassIdAsync(Guid academicClassId);
        Task<int> BulkUpdateEnrollmentStatusByClassIdsAsync(List<Guid> classIds, int newStatus);
        Task<int> StartEnrollmentsByAcademicClassIdAsync(Guid classId);
    }
}
