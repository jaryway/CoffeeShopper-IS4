using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;
using DataAccess.Entities;

namespace DataAccess.Data
{
    public class EntityTypeGenerator
    {

        public static void RegisterEntities(ModelBuilder modelBuilder)
        {
            var generatedTypes = GetGeneratedEntityTypes();
            foreach (var type in generatedTypes)
            {
                var configureMethod = type.GetMethod("Configure", new[] { typeof(EntityTypeBuilder<>) });
                if (configureMethod != null)
                {
                    var builder = (dynamic)Activator.CreateInstance(type);
                    configureMethod.Invoke(builder, new[] { modelBuilder.Entity(type) });
                }
                else
                {
                    modelBuilder.Entity(type);
                }
            }
        }

        private static IEnumerable<Type> GetGeneratedEntityTypes()
        {
            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Select(Assembly.LoadFrom)
                .ToArray();

            return assemblies.SelectMany(a => a.GetTypes())
                .Where(t => typeof(GeneratedEntityBase).IsAssignableFrom(t) && !t.IsAbstract);
        }
    }
}

