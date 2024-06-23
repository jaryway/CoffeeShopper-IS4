using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Runtime.Loader;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
//using DynamicDbContextAssembly.Models;
namespace DynamicDbContextAssembly;

public abstract class EntityBase { }

public class DynamicDbContext : DbContext
{
    private readonly Assembly? assembly;
    public DynamicDbContext(DbContextOptions<DynamicDbContext> options, Assembly assembly) : base(options)
    {
        this.assembly = assembly;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        if (this.assembly != null)
        {
            var entityTypes = this.assembly
                 .GetTypes()
                 .Where(t => typeof(EntityBase).IsAssignableFrom(t) && !t.IsAbstract)
                 .ToList();

            foreach (var t in entityTypes)
            {
                modelBuilder.Entity(t).ToTable(t.Name);
            }
        }

        base.OnModelCreating(modelBuilder);

    }
}