using Ecommerce.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data.EF
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Token> Tokens => Set<Token>();
        //public DbSet<ChatRoom> ChatRooms => Set<ChatRoom>();
        //public DbSet<Message> Messages => Set<Message>();
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasIndex(e => e.CategoryName).IsUnique(true);
                entity.Property(e => e.CategoryName).IsRequired();
            });

            builder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasIndex(e => e.ProductName).IsUnique(true);
                entity.Property(e => e.ProductName).IsRequired();

                entity.Property(e => e.Price).HasColumnType("decimal(18,4)").HasDefaultValue(0);

                entity.Property(e => e.Rating).HasColumnType("decimal(18,4)").HasDefaultValue(0);
                entity.Property(e => e.Quantity).HasDefaultValue(0);
                
                entity.HasOne(e => e.Category)
                .WithMany(category => category.Products)
                .HasForeignKey(e => e.CategoryId);

            });

            builder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasOne(e => e.Customer)
                .WithMany(user => user.Addresses)
                .HasForeignKey(e => e.CustomerId);
            });

            builder.Entity<CartItem>().HasKey(p => new { p.ProductId, p.CustomerId });

            builder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasOne(e => e.Customer)
                .WithMany(user => user.Orders)
                .HasForeignKey(e => e.CustomerId);

                entity.HasIndex(e => e.OrderId).IsUnique(true);
            });

            builder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(p => new { p.OrderId, p.ProductId });
                entity.Property(e => e.Price)
                .HasColumnType("decimal(18,4)");
                entity.Property(e => e.Total)
                .HasColumnType("decimal(18,4)");
            });

            builder.Entity<ProductImage>().HasKey(p => new { p.ProductId, p.ImageUrl });

            builder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.Customer)
                .WithMany(user => user.Reviews)
                .HasForeignKey(e => e.CustomerId);

                entity.HasOne(e => e.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(e => e.ProductId);
                entity.Property(e => e.Rating).HasColumnType("decimal(18,4)");
            });

            builder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.Customer)
                .WithMany(user => user.Comments)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Review)
                .WithMany(x => x.Comments)
                .HasForeignKey(e => e.ReviewId);
            });
        }
    }
}