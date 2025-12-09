using Microsoft.EntityFrameworkCore;

namespace TanuloNaplo;

public class NaploContext : DbContext
{
    // Db tábla 
    public DbSet<UserNote> Notes { get; set; }

    // SQLlite használaa 
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=tanulonaplo.db");
    }
}