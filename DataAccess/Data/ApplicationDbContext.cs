using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
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
