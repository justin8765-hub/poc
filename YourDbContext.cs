using Microsoft.EntityFrameworkCore;

public class YourDbContext : DbContext
{
    public DbSet<Item> Items { get; set; }
    public DbSet<ChangeLog> ChangeLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionString");
    }
}
