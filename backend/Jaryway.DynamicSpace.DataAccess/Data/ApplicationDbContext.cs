using Jaryway.DynamicSpace.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jaryway.DynamicSpace.DataAccess.Data
{
    public partial class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{ }

		public DbSet<CoffeeShop> CoffeeShops { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    //EntityTypeGenerator.RegisterEntities(modelBuilder);
        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
