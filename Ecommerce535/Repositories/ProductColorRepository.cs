using Ecommerce535.Data;

namespace Ecommerce535.Repositories
{
    public class ProductColorRepository : Repository<ProductColor> , IProductColorRepository
    {
        public ProductColorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void RemoveRange(IEnumerable<ProductColor> productColors)
        {
            _context.RemoveRange(productColors);
        }
    }
}
