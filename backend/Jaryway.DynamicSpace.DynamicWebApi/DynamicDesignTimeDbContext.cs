
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Jaryway.DynamicSpace.DynamicWebApi.Attributes;
using Jaryway.DynamicSpace.DynamicWebApi.Entities;
using Microsoft.EntityFrameworkCore;


namespace Jaryway.DynamicSpace.DynamicWebApi
{
    public class DynamicDesignTimeDbContext : DynamicDbContextBase
    {
        public DynamicDesignTimeDbContext(DbContextOptions<DynamicDesignTimeDbContext> options) : base(options)
        {
        }

        protected override DynamicAssemblyBuilder InitializeDynamicAssemblyBuilder() => DynamicAssemblyBuilder.GetInstance(true);
    }

}
