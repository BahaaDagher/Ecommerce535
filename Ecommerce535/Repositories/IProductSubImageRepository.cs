using Microsoft.EntityFrameworkCore;

namespace Ecommerce535.Repositories
{
    public interface IProductSubImageRepository : IRepository<ProductSubImage>
    { 
        void RemoveRange(IEnumerable<ProductSubImage> productSubImages);
    }
}
