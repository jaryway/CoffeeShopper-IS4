using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DynamicSpace
{
    public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context, bool designTime)
        {
            return context is DynamicDesignTimeDbContext dynamicDbContext
            ? (context.GetType(), dynamicDbContext.Assembly, designTime)
            : (object)context.GetType();
        }
    }
}

