using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Jaryway.DynamicSpace.DynamicWebApi
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

            return context.GetType();
        }
    }
}

