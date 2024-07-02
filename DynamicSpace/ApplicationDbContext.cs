using DynamicSpace.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicSpace;
public class ApplicationDbContext : DbContext
{
    //private bool isDynamicMode = true;

    public ApplicationDbContext()
    { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

        //this.Set(typeof(Models.Test));

        //this.Set(typeof(DynamicClass), "");
        //DynamicClasses.Remove();

    }

    public DbSet<DynamicClass> DynamicClasses { get; set; }

    public DbSet<MigrationEntry> MigrationEntries { get; set; }

    public IQueryable<T> Query<T>() where T : class
    {
        return Set<T>() as IQueryable<T>;
    }

    //public DbSet<Test> Tests { get; set; }

    //public DbSet<SourceCode> SourceCodes { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    //base.OnConfiguring(optionsBuilder);
    //    //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");

    //    //var serverVersion = new MySqlServerVersion(Version.Parse("8.0.0"));
    //    //var connectionString = "server=localhost;uid=root;pwd=123456;database=test";
    //    //optionsBuilder.UseMySql(connectionString, serverVersion);
    //    //optionsBuilder.ReplaceService<IMigrationsAssembly, DynamicMigrationsAssembly>();
    //}
    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    base.OnModelCreating(modelBuilder);
    //}
}


