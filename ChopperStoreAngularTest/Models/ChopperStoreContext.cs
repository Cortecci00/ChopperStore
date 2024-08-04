using Microsoft.EntityFrameworkCore;

namespace ChopperStoreAngularTest.Models
{
    public class ChopperStoreContext : DbContext
    {
        public DbSet<Category> categories { get; set; }
        public DbSet<Item> items { get; set; }
        public DbSet<ShoppingCart> shoppingcarts { get; set; }
        public DbSet<Transaction> transactions { get; set; }
        public DbSet<Skin> skins { get; set; }
        public DbSet<User> users { get; set; }

        public ChopperStoreContext(DbContextOptions options) : base(options) 
        { 
        }

    }
}
