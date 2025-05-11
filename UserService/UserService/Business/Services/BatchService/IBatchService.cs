using UserService.Business.Dtos.Batch;
using UserService.Entities;

namespace UserService.Business.Services.BatchService
{
    public interface IBatchService
    {
        Task<List<BatchReadDto>> GetAllBatches();
        Task<BatchReadDto> GetBatchById(Guid id);
        Task<BatchReadDto> CreateBatch(BatchCreateDto batchDto);
        Task<BatchReadDto> UpdateBatch(Guid id, BatchUpdateDto batchDto);
        Task<bool> DeleteBatch(Guid id);
    }
} 