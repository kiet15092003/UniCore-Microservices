using Microsoft.EntityFrameworkCore;
using UserService.Entities;

namespace UserService.DataAccess.Repositories.BatchRepo
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

        public async Task<List<Batch>> GetAllAsync()
        {
            return await _context.Batches.ToListAsync();
        }

        public async Task<Batch?> GetByIdAsync(Guid id)
        {
            return await _context.Batches.FindAsync(id);
        }

        public async Task<Batch> CreateAsync(Batch batch)
        {
            await _context.Batches.AddAsync(batch);
            await _context.SaveChangesAsync();
            return batch;
        }

        public async Task<Batch> UpdateAsync(Batch batch)
        {
            _context.Batches.Update(batch);
            await _context.SaveChangesAsync();
            return batch;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var batch = await _context.Batches.FindAsync(id);
            if (batch == null)
                return false;

            _context.Batches.Remove(batch);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
