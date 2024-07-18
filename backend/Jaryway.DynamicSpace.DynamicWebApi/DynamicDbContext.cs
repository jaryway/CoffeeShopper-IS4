
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Jaryway.DynamicSpace.DynamicWebApi.Attributes;
using Jaryway.DynamicSpace.DynamicWebApi.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jaryway.DynamicSpace.DynamicWebApi
{
    public class DynamicDbContext : DynamicDbContextBase
    {
        public DynamicDbContext(DbContextOptions<DynamicDbContext> options) : base(options)
        {

        }

        protected override DynamicAssemblyBuilder InitializeDynamicAssemblyBuilder() => DynamicAssemblyBuilder.GetInstance();
    }

}
