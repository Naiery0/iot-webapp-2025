using Microsoft.EntityFrameworkCore;

namespace WebApiApp01.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        protected AppDbContext()
        {

        }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
