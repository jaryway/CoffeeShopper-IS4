using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DynamicSpace
{
    public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context, bool designTime)
        {
            if (context is DynamicDesignTimeDbContext dynamicDesignTimeDbContext)
            {
                return (context.GetType(), dynamicDesignTimeDbContext.Assembly, designTime);
            }

            if (context is DynamicDbContext dynamicDbContext)
            {
                return (context.GetType(), dynamicDbContext.Assembly);
            }

            return (object)context.GetType();

        }
    }
}

