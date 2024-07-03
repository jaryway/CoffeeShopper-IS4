
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using DynamicSpace.Attributes;
using DynamicSpace.Models;
using Microsoft.EntityFrameworkCore;


namespace DynamicSpace
{
    public class DynamicDesignTimeDbContext : DynamicDbContextBase
    {
        public DynamicDesignTimeDbContext(DbContextOptions<DynamicDesignTimeDbContext> options) : base(options)
        {
        }

        protected override DynamicAssemblyBuilder InitializeDynamicAssemblyBuilder() => DynamicAssemblyBuilder.GetInstance(true);
    }

}
