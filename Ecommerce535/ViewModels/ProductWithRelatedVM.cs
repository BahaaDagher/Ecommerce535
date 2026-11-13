using System.Collections;

namespace Ecommerce535.ViewModels
{
    public class ProductWithRelatedVM
    {
        public Product Product { get; set; }
        public IEnumerable<Product> RelatedProducts { get; set; }
    }
}
