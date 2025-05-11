using AutoMapper;
using StudentService.Business.Dtos.Batch;
using StudentService.DataAccess.Repositories.BatchRepo;
using StudentService.Entities;

namespace StudentService.Business.Services.BatchService
{
    public class BatchSvc : IBatchSvc
    {
        private readonly IBatchRepo _batchRepo;
        private readonly IMapper _mapper;

        public BatchSvc(IBatchRepo batchRepo, IMapper mapper)
        {
            _batchRepo = batchRepo;
            _mapper = mapper;
        }

        public async Task<List<BatchReadDto>> GetAllBatches()
        {
            var batches = await _batchRepo.GetAllAsync();
            return _mapper.Map<List<BatchReadDto>>(batches);
        }

        public async Task<BatchReadDto> GetBatchById(Guid id)
        {
            var batch = await _batchRepo.GetByIdAsync(id);
            if (batch == null)
                throw new KeyNotFoundException($"Batch with ID {id} not found");

            return _mapper.Map<BatchReadDto>(batch);
        }

        public async Task<BatchReadDto> CreateBatch(BatchCreateDto batchDto)
        {
            var batch = _mapper.Map<Batch>(batchDto);
            var createdBatch = await _batchRepo.CreateAsync(batch);
            return _mapper.Map<BatchReadDto>(createdBatch);
        }

        public async Task<BatchReadDto> UpdateBatch(Guid id, BatchUpdateDto batchDto)
        {
            var existingBatch = await _batchRepo.GetByIdAsync(id);
            if (existingBatch == null)
                throw new KeyNotFoundException($"Batch with ID {id} not found");

            _mapper.Map(batchDto, existingBatch);
            var updatedBatch = await _batchRepo.UpdateAsync(existingBatch);
            return _mapper.Map<BatchReadDto>(updatedBatch);
        }

        public async Task<bool> DeleteBatch(Guid id)
        {
            var result = await _batchRepo.DeleteAsync(id);
            if (!result)
                throw new KeyNotFoundException($"Batch with ID {id} not found");

            return result;
        }
    }
} 