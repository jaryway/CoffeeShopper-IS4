using Jaryway.DynamicSpace.DynamicWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Jaryway.DynamicSpace.DynamicWebApi;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<DynamicClass> DynamicClasses { get; set; }

    public DbSet<MigrationEntry> MigrationEntries { get; set; }

}


