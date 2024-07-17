using System;
namespace Jaryway.DynamicSpace.DynamicWebApi.Design
{
    public interface IDynamicMigrator
    {
        MigrateReuslt Migrate(string? targetMigration = null);
    }
}

