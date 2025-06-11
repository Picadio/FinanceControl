using FinanceControl.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Database;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
 
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
}