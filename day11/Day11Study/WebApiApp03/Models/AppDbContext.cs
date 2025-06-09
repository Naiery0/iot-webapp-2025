using Microsoft.EntityFrameworkCore;

namespace WebApiApp03.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        protected AppDbContext()
        {

        }

        public DbSet<iot_datas> iot_datas { get; set; }
    }
}
