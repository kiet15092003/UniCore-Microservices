using CourseService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseService.DataAccess.Repositories
{
    public class CourseMaterialRepository : ICourseMaterialRepository
    {
        private readonly AppDbContext _context;

        public CourseMaterialRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CourseMaterial> GetByIdAsync(Guid courseId, Guid materialId)
        {
            return await _context.CourseMaterials
                .Include(cm => cm.Material)
                .FirstOrDefaultAsync(cm => cm.CourseId == courseId && cm.MaterialId == materialId);
        }

        public async Task<IEnumerable<CourseMaterial>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.CourseMaterials
                .Include(cm => cm.Material)
                .Where(cm => cm.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<CourseMaterial> AddAsync(CourseMaterial courseMaterial)
        {
            await _context.CourseMaterials.AddAsync(courseMaterial);
            await _context.SaveChangesAsync();
            return courseMaterial;
        }

        public async Task<bool> UpdateAsync(CourseMaterial courseMaterial)
        {
            _context.CourseMaterials.Update(courseMaterial);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid courseId, Guid materialId)
        {
            var courseMaterial = await _context.CourseMaterials
                .FirstOrDefaultAsync(cm => cm.CourseId == courseId && cm.MaterialId == materialId);
            
            if (courseMaterial == null)
                return false;

            _context.CourseMaterials.Remove(courseMaterial);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Material> GetMaterialByIdAsync(Guid materialId)
        {
            return await _context.Materials.FindAsync(materialId);
        }

        public async Task<Material> AddMaterialAsync(Material material)
        {
            await _context.Materials.AddAsync(material);
            await _context.SaveChangesAsync();
            return material;
        }

        public async Task<bool> UpdateMaterialAsync(Material material)
        {
            _context.Materials.Update(material);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteMaterialAsync(Guid materialId)
        {
            var material = await _context.Materials.FindAsync(materialId);
            if (material == null)
                return false;

            // Kiểm tra xem material có được sử dụng bởi course nào không
            var isUsed = await _context.CourseMaterials.AnyAsync(cm => cm.MaterialId == materialId);
            if (isUsed)
                return false;

            _context.Materials.Remove(material);
            return await _context.SaveChangesAsync() > 0;
        }
    }
} 