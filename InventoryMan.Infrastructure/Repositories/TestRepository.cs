using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using InventoryMan.Infrastructure.Data.Context;

namespace InventoryMan.Infrastructure.Repositories
{
    public class TestRepository : BaseRepository<Test>, ITestRepository
    {
        public TestRepository(InventoryDbContext context) : base(context)
        {
        }
    }
}
