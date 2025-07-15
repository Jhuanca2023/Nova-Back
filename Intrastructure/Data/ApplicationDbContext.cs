using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Intrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<Users>
    {
        public DbSet<Users> Users { get; set; }

        // public DbSet<Role> Roles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductComment> ProductComments { get; set; }
        public DbSet<CartShop> CartShops { get; set; }
        public DbSet<CartShopDetail> CartShopDetails { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // modelBuilder.Entity<IdentityUser>()
            //     .Ignore(u => u.UserName)
            //     ;
            modelBuilder.Entity<Users>()
                // .Ignore(u => u.UserName)
                // .Ignore(u => u.PhoneNumber)
                .Ignore(u => u.SecurityStamp)
                .Ignore(u => u.PhoneNumberConfirmed)
                // .Ignore(u => u.PasswordHash)
                .Ignore(u => u.TwoFactorEnabled)
                .Ignore(u => u.AccessFailedCount);

            //Cambio de nombres para las tablas de Identity
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");


            // modelBuilder.Entity<User>()
            //     .HasOne(u => u.Role)
            //     .WithMany(r => r.Users)
            //     .HasForeignKey(u => u.RoleId);

            // Índice para Product.CategoryId.
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CategoryId);

            // Índice para Order.UserId.
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.UserId);

            // Índice compuesto en ProductComment (ProductId, UserId).
            modelBuilder.Entity<ProductComment>()
                .HasIndex(pc => new { pc.ProductId, pc.UserId })
                .IsUnique() //-> Si solo queremos que el usuario tenga un comentario por producto
                ;

            // Índice para Cart.UserId.
            modelBuilder.Entity<CartShop>(entity =>
            {
                entity.HasIndex(c => c.UserId);
                entity.Property(c => c.UserId).HasColumnType("nvarchar(450)");

                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // Índice para CartDetail.CartId.
            modelBuilder.Entity<CartShopDetail>()
                .HasIndex(cd => cd.CartId);

            // Índice para Transaction.PaymentDate.
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.PaymentDate);

            // Índice para Order.Status.
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Status);

            // Índice para Product.Price.
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Price);
            modelBuilder.Entity<Favorite>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<Favorite>()
                .HasIndex(f => new { f.UserId, f.ProductId })
                .IsUnique();

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Product)
                .WithMany()
                .HasForeignKey(f => f.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}