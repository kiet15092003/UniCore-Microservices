using Microsoft.AspNetCore.Mvc;
using StudentService.Business.Dtos.Batch;
using StudentService.Business.Services.BatchService;
using StudentService.Middleware;

namespace StudentService.Controller
{
    [Route("api/s/[controller]")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly IBatchSvc _batchSvc;

        public BatchController(IBatchSvc batchSvc)
        {
            _batchSvc = batchSvc;
        }

        [HttpGet]
        public async Task<ApiResponse<List<BatchReadDto>>> GetAllBatches()
        {
            var batches = await _batchSvc.GetAllBatches();
            return ApiResponse<List<BatchReadDto>>.SuccessResponse(batches);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<BatchReadDto>> GetBatchById(Guid id)
        {
            try
            {
                var batch = await _batchSvc.GetBatchById(id);
                return ApiResponse<BatchReadDto>.SuccessResponse(batch);
            }
            catch (KeyNotFoundException ex)
            {
                return ApiResponse<BatchReadDto>.ErrorResponse([ex.Message]);
            }
        }

        [HttpPost]
        public async Task<ApiResponse<BatchReadDto>> CreateBatch([FromBody] BatchCreateDto batchDto)
        {
            try
            {
                var batch = await _batchSvc.CreateBatch(batchDto);
                return ApiResponse<BatchReadDto>.SuccessResponse(batch);
            }
            catch (Exception ex)
            {
                return ApiResponse<BatchReadDto>.ErrorResponse([$"Error creating batch: {ex.Message}"]);
            }
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<BatchReadDto>> UpdateBatch(Guid id, [FromBody] BatchUpdateDto batchDto)
        {
            try
            {
                var batch = await _batchSvc.UpdateBatch(id, batchDto);
                return ApiResponse<BatchReadDto>.SuccessResponse(batch);
            }
            catch (KeyNotFoundException ex)
            {
                return ApiResponse<BatchReadDto>.ErrorResponse([ex.Message]);
            }
            catch (Exception ex)
            {
                return ApiResponse<BatchReadDto>.ErrorResponse([$"Error updating batch: {ex.Message}"]);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteBatch(Guid id)
        {
            try
            {
                var result = await _batchSvc.DeleteBatch(id);
                return ApiResponse<bool>.SuccessResponse(result);
            }
            catch (KeyNotFoundException ex)
            {
                return ApiResponse<bool>.ErrorResponse([ex.Message]);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse([$"Error deleting batch: {ex.Message}"]);
            }
        }
    }
} 