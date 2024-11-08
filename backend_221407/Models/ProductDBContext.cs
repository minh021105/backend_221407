using Microsoft.EntityFrameworkCore;

namespace backend_221407.Models
{
    public class ProductDBContext: DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder opt)
        {
            String conn = Global.getConnectString();
            opt.UseSqlServer(conn);

        }
    }
}
