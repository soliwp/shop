using Microsoft.EntityFrameworkCore;
using shop_file_upload_.Models;

namespace shop_file_upload_
{
    public class shopDbContext : DbContext
    {
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImages> productImages { get; set; }        

        public shopDbContext(DbContextOptions<shopDbContext> options) : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductCategory>().HasData(
                new()
                {
                    Id = 1,
                    CategoryName = "کالای دیجیتال",
                    Parent = null,
                },
                new()
                {
                    Id = 2,
                    CategoryName = "لپتاپ",
                    parentId = 1,
                },
                new()
                {
                    Id = 3,
                    CategoryName = "فیلم",
                    parentId = null,
                }
                );
        }
    }
}
