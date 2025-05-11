using StudentService.Business.Dtos.Batch;
using StudentService.Entities;

namespace StudentService.Business.Services.BatchService
{
    public interface IBatchSvc
    {
        Task<List<BatchReadDto>> GetAllBatches();
        Task<BatchReadDto> GetBatchById(Guid id);
        Task<BatchReadDto> CreateBatch(BatchCreateDto batchDto);
        Task<BatchReadDto> UpdateBatch(Guid id, BatchUpdateDto batchDto);
        Task<bool> DeleteBatch(Guid id);
    }
} 