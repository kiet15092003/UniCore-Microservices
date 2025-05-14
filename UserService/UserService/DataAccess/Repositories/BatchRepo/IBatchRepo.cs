using UserService.Entities;

namespace UserService.DataAccess.Repositories.BatchRepo
{
    public interface IBatchRepo
    {
        Task<Batch> GetBatchByIdAsync(Guid Id);
        Task<List<Batch>> GetAllAsync();
        Task<Batch?> GetByIdAsync(Guid id);
        Task<Batch> CreateAsync(Batch batch);
        Task<Batch> UpdateAsync(Batch batch);
        Task<bool> DeleteAsync(Guid id);
    }
}
