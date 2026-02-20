using Microsoft.EntityFrameworkCore;

namespace StudentSpot.Models
{
    public class myData : DbContext
    {
        public myData(DbContextOptions<myData> options) : base(options)
        {
        }
        public DbSet<Admin> tbl_admin { get; set; }
        public DbSet<Customer> tbl_customer { get; set; }

        public DbSet<Product> tbl_product { get; set; }
        public DbSet<Category> tbl_category { get; set; }
        public DbSet<Bestblog> tbl_bestblog { get; set; }
        public DbSet<Cart> tbl_cart { get; set; }
        public DbSet<Feedback> tbl_feedback { get; set; }
        public DbSet<Wishlist> tbl_wishlist { get; set; } // ✅ add this
        public DbSet<BayNow> tbl_baynow { get; set; }

        //public DbSet<Order> tbl_order { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasOne(p => p.Category).WithMany(c => c.product).HasForeignKey(p => p.cat_id);
        }


    }

}

   