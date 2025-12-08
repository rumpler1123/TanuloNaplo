using Microsoft.EntityFrameworkCore;

namespace TanuloNaplo;

public class NaploContext : DbContext
{
    // Ez a tábla lesz az adatbázisban
    public DbSet<UserNote> Notes { get; set; }

    // Itt mondjuk meg, hogy SQLite-ot használunk
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=tanulonaplo.db");
    }
}