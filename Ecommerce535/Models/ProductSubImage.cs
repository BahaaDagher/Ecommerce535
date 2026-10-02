using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce535.Models
{
    [PrimaryKey(nameof(Img) , nameof(ProductId))]
    public class ProductSubImage
    {
        public string Img { get; set; }
        public int ProductId{ get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }
}
