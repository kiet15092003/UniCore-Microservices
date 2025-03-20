using StudentService.Entities;

namespace StudentService.DataAccess.Repositories.BatchRepo
{
    public interface IBatchRepo
    {
        Task<Batch> GetBatchByIdAsync(Guid Id);
    }
}
