using E_Commers_Adelia.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Internal;
using System.Reflection.Emit;
using E_Commers_Adelia.Common;

namespace E_Commers_Adelia.Data
{
    public class ApplicationDbContext : IdentityDbContext<EUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductOption> ProductOptions { get; set; }
        public DbSet<QrCode> QrCodes { get; set; }
        public DbSet<SellerDeliveryOption> SellerDeliveryOptions { get; set; }
        public DbSet<SellerPaymentMethod> SellerPaymentMethods { get; set; }
        public DbSet<CodNote> CodNotes { get; set; }
        public DbSet<CashNote> CashNotes { get; set; }
        public DbSet<OnlineTransferNote> onlineTransferNotes { get; set; }
        public DbSet<SelfPickupAddress> selfPickupAddresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 🔗 Relationship: Product → ProductOption (1 to many)
            builder.Entity<Product>()
                .HasMany(p => p.Options)
                .WithOne(o => o.Product)
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
