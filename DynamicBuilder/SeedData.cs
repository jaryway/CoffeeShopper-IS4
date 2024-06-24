using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicBuilder
{
    public class SeedData
    {
        public static void Initialize(ApplicationDbContext dbContext)
        {
            if (!dbContext.SourceCodes.Any())
            {
                foreach (var entity in Config.SourceCodes.ToList())
                {
                    dbContext.SourceCodes.Add(entity);
                }

                dbContext.SaveChanges();
            }
        }
    }
}
