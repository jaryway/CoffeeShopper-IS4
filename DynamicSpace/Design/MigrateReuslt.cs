using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DynamicSpace.Design
{
    public class MigrateReuslt
    {
        public MigrateReuslt(IReadOnlyList<Migration> migrationsToApply, IReadOnlyList<Migration> migrationsToRevert)
        {
            MigrationsToApply = migrationsToApply;
            MigrationsToRevert = migrationsToRevert;
        }
        public IReadOnlyList<Migration> MigrationsToApply { get; set; } = [];
        public IReadOnlyList<Migration> MigrationsToRevert { get; set; } = [];
    }
}
