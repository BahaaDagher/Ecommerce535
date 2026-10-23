using Ecommerce535.Data;

namespace Ecommerce535.Repositories
{
    public class ProductSubImageRepository : Repository<ProductSubImage>  , IProductSubImageRepository
    {
        public ProductSubImageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void RemoveRange(IEnumerable<ProductSubImage> productSubImages)
        {
            _context.RemoveRange(productSubImages);
        }
    }
}
