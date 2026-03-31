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
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<ProductImage> ProductsImages { get; set; }
        public virtual DbSet<ProductAttribute> ProductAttributes { get; set; }
        public virtual DbSet<ProductReview> ProductReviews { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }

        /// <summary>
        /// ✅ OnModelCreating - Tüm FK ilişkileri ve constraint'leri yapılandırıyoruz
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================================
            // CART-CARTITEM İLİŞKİSİ (One-to-Many)
            // ============================================
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================================
            // PRODUCT-CARTITEM İLİŞKİSİ (One-to-Many)
            // ============================================
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================================
            // CUSTOMER-CART İLİŞKİSİ (One-to-Many)
            // ============================================
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Customer)
                .WithMany()
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================================
            // PRODUCT-PRODUCTIMAGE İLİŞKİSİ (One-to-Many)
            // ============================================
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================================
            // PRODUCT-PRODUCTATTRIBUTE İLİŞKİSİ (One-to-Many)
            // ============================================
            modelBuilder.Entity<ProductAttribute>()
                .HasOne(pa => pa.Product)
                .WithMany(p => p.Attributes)
                .HasForeignKey(pa => pa.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================================
            // PRODUCT-PRODUCTREVIEW İLİŞKİSİ (One-to-Many)
            // ✅ NOT: ProductReview.ProductId INT olmalı!
            // ============================================
            modelBuilder.Entity<ProductReview>()
                .HasOne(pr => pr.Product)
                .WithMany(p => p.ProductReviews)
                .HasForeignKey(pr => pr.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================================
            // CUSTOMER-PRODUCTREVIEW İLİŞKİSİ (One-to-Many)
            // ============================================
            modelBuilder.Entity<ProductReview>()
                .HasOne(pr => pr.Customer)
                .WithMany()
                .HasForeignKey(pr => pr.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============================================
            // PRODUCT-BRAND İLİŞKİSİ (Many-to-One)
            // ============================================
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany()
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================================
            // PRODUCT-SUBCATEGORY İLİŞKİSİ (Many-to-One)
            // ============================================
            modelBuilder.Entity<Product>()
                .HasOne(p => p.SubCategory)
                .WithMany()
                .HasForeignKey(p => p.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================================
            // SUBCATEGORY-CATEGORY İLİŞKİSİ (Many-to-One)
            // ============================================
            modelBuilder.Entity<SubCategory>()
                .HasOne(sc => sc.Category)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(sc => sc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============================================
            // INDICES - PERFORMANS OPTİMİZASYONU
            // ============================================

            // Cart tablosunda her müşterinin sadece 1 sepeti olması için
            modelBuilder.Entity<Cart>()
                .HasIndex(c => c.CustomerId)
                .IsUnique();

            // CartItem tablosunda aynı ürün sepete iki kez eklenemesin
            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new { ci.CartId, ci.ProductId })
                .IsUnique();
        }
    }
}