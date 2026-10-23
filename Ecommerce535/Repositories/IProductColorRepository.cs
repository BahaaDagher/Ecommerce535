using Microsoft.EntityFrameworkCore;

namespace Ecommerce535.Repositories
{
    public interface IProductColorRepository : IRepository<ProductColor>
    {
        void RemoveRange(IEnumerable<ProductColor> productColors);
    }
}
