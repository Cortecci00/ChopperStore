using Microsoft.EntityFrameworkCore;

namespace ChopperStoreAngularTest.Models
{
    public class ChopperStoreContext : DbContext
    {
        public DbSet<Category> categories { get; set; }
        public DbSet<Item> items { get; set; }
        public DbSet<ShoppingCart> shoppingcarts { get; set; }
        public DbSet<Transaction> transactions { get; set; }
        public DbSet<TransactionItem> transactionItems { get; set; }
        public DbSet<Skin> skins { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Recommendation> recommendations { get; set; }
        public DbSet<ContactMessage> contactMessages { get; set; }

        public ChopperStoreContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShoppingCart>()
                .HasIndex(c => c.UserId)
                .IsUnique();

            modelBuilder.Entity<Skin>()
                .HasOne(s => s.category)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item>()
                .HasOne(i => i.skin)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransactionItem>()
                .HasOne(ti => ti.skin)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TransactionItem>()
                .HasOne(ti => ti.transaction)
                .WithMany(t => t.items)
                .HasForeignKey(ti => ti.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
