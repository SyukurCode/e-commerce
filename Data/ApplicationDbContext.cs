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
        public DbSet<CustomerDeliveryInfo> CustomerDeliveryInfo { get; set; }
        public DbSet<CustomerPayment> CustomerPayments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserChat> Chats { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 🔗 Relationship: Product → ProductOption (1 to many)
            builder.Entity<Product>()
                .HasMany(p => p.Options)
                .WithOne(o => o.Product)
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            // 🔗 Avatar
            builder.Entity<Avatar>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // 🔗 Product
            builder.Entity<Product>()
                .HasOne(o => o.User)
                .WithMany() // kalau nak simpan senarai order dalam user, boleh letak navigation property
                .HasForeignKey(o => o.userId)
                .OnDelete(DeleteBehavior.Cascade);
            // 🔗 QRCode
            builder.Entity<QrCode>()
                .HasOne(q => q.User)
                .WithMany()
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // 🔗 Online Transfer
            builder.Entity<OnlineTransferNote>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // 🔗 COD Note
            builder.Entity<CodNote>()
               .HasOne(o => o.User)
               .WithMany()
               .HasForeignKey(o => o.UserId)
               .OnDelete(DeleteBehavior.Cascade);
            // 🔗 Cash Note
            builder.Entity<CashNote>()
               .HasOne(o => o.User)
               .WithMany()
               .HasForeignKey(o => o.UserId)
               .OnDelete(DeleteBehavior.Cascade);
            // 🔗 Delivery Option
            builder.Entity<SellerDeliveryOption>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.userId)
                .OnDelete(DeleteBehavior.Cascade);
            // 🔗 Selftpickup address
            builder.Entity<SelfPickupAddress>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // 🔗 Paymnet Option
            builder.Entity<SellerPaymentMethod>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // 🔗 User Chat
            builder.Entity<UserChat>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
