using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Api.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
