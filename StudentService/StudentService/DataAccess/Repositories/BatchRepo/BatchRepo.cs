using Microsoft.EntityFrameworkCore;
using StudentService.Entities;

namespace StudentService.DataAccess.Repositories.BatchRepo
{
    public class BatchRepo : IBatchRepo
    {
        private readonly AppDbContext _context;
        public BatchRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Batch> GetBatchByIdAsync(Guid Id)
        {
            var result = await _context.Batches.FirstOrDefaultAsync(d => d.Id == Id);
            if (result == null)
            {
                throw new KeyNotFoundException("Batch not found");
            }
            return result;
        }
    }
}
