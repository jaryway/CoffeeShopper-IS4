using Jaryway.DynamicSpace.DynamicWebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jaryway.DynamicSpace.DynamicWebApi;
/// <summary>
/// 
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// 
    /// </summary>
    public ApplicationDbContext()
    { }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }
    /// <summary>
    /// 
    /// </summary>
    public DbSet<DynamicClass> DynamicClasses { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public DbSet<MigrationEntry> MigrationEntries { get; set; }
}


