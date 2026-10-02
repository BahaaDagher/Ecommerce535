
using Microsoft.EntityFrameworkCore;

namespace Ecommerce535.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; } 
        public DbSet<Brand> Brands { get; set; } 
        public DbSet<Product> Products { get; set; } 
        public DbSet<ProductSubImage> ProductSubImages { get; set; } 
        public DbSet<ProductColor> ProductColors { get; set; } 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=.;initial catalog = Ecommerce535;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;"); 
        }
    }
}
