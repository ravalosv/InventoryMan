using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using InventoryMan.Infrastructure.Data.Context;
using InventoryMan.Infrastructure.Data.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventoryMan.Infrastructure.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(InventoryDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _dbSet.Where(p => p.CategoryId == categoryId).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _dbSet.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            return await _dbSet
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }


    }
}
