using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;
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

        private IQueryable<Material> ApplyFilters(IQueryable<Material> queryable, MaterialListFilterParams filterParams)
        {
            // Lọc theo tên
            if (!string.IsNullOrWhiteSpace(filterParams.Name))
            {
                queryable = queryable.Where(m => m.Name.Contains(filterParams.Name));
            }

            // Lọc theo loại tài liệu
            if (filterParams.MaterialTypeId.HasValue)
            {
                queryable = queryable.Where(m => m.MaterialTypeId == filterParams.MaterialTypeId.Value);
            }

            // Lọc theo có file hay không
            if (filterParams.HasFile.HasValue)
            {
                if (filterParams.HasFile.Value)
                {
                    queryable = queryable.Where(m => !string.IsNullOrEmpty(m.FileUrl));
                }
                else
                {
                    queryable = queryable.Where(m => string.IsNullOrEmpty(m.FileUrl));
                }
            }

            return queryable;
        }

        private IQueryable<Material> ApplySorting(IQueryable<Material> queryable, Order? order)
        {
            if (order != null && !string.IsNullOrEmpty(order.By))
            {
                switch (order.By.ToLower())
                {
                    case "name":
                        queryable = order.IsDesc 
                            ? queryable.OrderByDescending(m => m.Name)
                            : queryable.OrderBy(m => m.Name);
                        break;
                    case "createdat":
                        queryable = order.IsDesc 
                            ? queryable.OrderByDescending(m => m.CreatedAt)
                            : queryable.OrderBy(m => m.CreatedAt);
                        break;
                    case "updatedat":
                        queryable = order.IsDesc 
                            ? queryable.OrderByDescending(m => m.UpdatedAt)
                            : queryable.OrderBy(m => m.UpdatedAt);
                        break;
                    default:
                        queryable = queryable.OrderByDescending(m => m.CreatedAt);
                        break;
                }
            }
            else
            {
                // Mặc định sắp xếp theo thời gian tạo mới nhất
                queryable = queryable.OrderByDescending(m => m.CreatedAt);
            }

            return queryable;
        }

        public async Task<PaginationResult<Material>> GetMaterialsPaginationAsync(
            Guid courseId,
            Pagination pagination,
            MaterialListFilterParams filterParams,
            Order? order)
        {
            // Lấy danh sách materialId từ CourseMaterial
            var materialIds = await _context.CourseMaterials
                .Where(cm => cm.CourseId == courseId)
                .Select(cm => cm.MaterialId)
                .ToListAsync();

            // Tạo query với các material thuộc course
            var queryable = _context.Materials
                .Include(m => m.MaterialType)
                .Where(m => materialIds.Contains(m.Id))
                .AsQueryable();

            // Áp dụng các bộ lọc
            queryable = ApplyFilters(queryable, filterParams);

            // Đếm tổng số lượng trước khi phân trang
            int total = await queryable.CountAsync();

            // Áp dụng sắp xếp
            queryable = ApplySorting(queryable, order);

            // Phân trang
            var result = await queryable
                .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                .Take(pagination.ItemsPerpage)
                .ToListAsync();

            return new PaginationResult<Material>
            {
                Data = result,
                Total = total,
                PageIndex = pagination.PageNumber,
                PageSize = pagination.ItemsPerpage
            };
        }

        public async Task<CourseMaterial> GetByIdAsync(Guid courseId, Guid materialId)
        {
            return await _context.CourseMaterials
                .Include(cm => cm.Material)
                .ThenInclude(m => m.MaterialType)
                .FirstOrDefaultAsync(cm => cm.CourseId == courseId && cm.MaterialId == materialId);
        }

        public async Task<IEnumerable<CourseMaterial>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.CourseMaterials
                .Include(cm => cm.Material)
                .ThenInclude(m => m.MaterialType)
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
            return await _context.Materials
                .Include(m => m.MaterialType)
                .FirstOrDefaultAsync(m => m.Id == materialId);
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