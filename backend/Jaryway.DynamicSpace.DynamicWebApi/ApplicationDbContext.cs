using Jaryway.DynamicSpace.DynamicWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Jaryway.DynamicSpace.DynamicWebApi;
public class ApplicationDbContext : DbContext
{
    //private bool isDynamicMode = true;

    public ApplicationDbContext()
    { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        //TimeSpan.Re
    }

    public DbSet<DynamicClass> DynamicClasses { get; set; }

    public DbSet<MigrationEntry> MigrationEntries { get; set; }

    public IQueryable<T> Query<T>() where T : class
    {
        return Set<T>();
    }


}


