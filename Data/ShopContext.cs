
using Core.Concretes.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace Data
{
    public class ShopContext : IdentityDbContext<Customer, IdentityRole, string>
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options)
        {
           
        }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Cart> Carts    { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<ProductImage> ProductsImages { get; set; }
        public virtual DbSet<ProductAttribute> ProductAttributes { get; set; }
        public virtual DbSet<ProductReview> ProductReviews { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }

    }
}
