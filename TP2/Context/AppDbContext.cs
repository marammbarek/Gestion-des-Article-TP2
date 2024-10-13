
using Microsoft.EntityFrameworkCore;
using TP2.Models;

namespace TP2.Context
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
    }
}
