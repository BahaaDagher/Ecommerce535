using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce535.Models
{
    [PrimaryKey(nameof(Color), nameof(ProductId))]

    public class ProductColor
    {
        public string Color { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }
}
