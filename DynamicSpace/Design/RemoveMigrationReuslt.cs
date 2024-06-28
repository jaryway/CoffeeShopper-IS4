using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DynamicSpace.Design
{
    public class RemoveMigrationReuslt
    {

        public string MigrationId { get; set; }
        public string MigrationMetadataId { get; set; }
        public string ModelSnapshotName { get; set; }
        public string ModelSnapshotCode { get; set; }
        public IReadOnlyList<Migration> MigrationsToApply { get; set; } = [];
        public IReadOnlyList<Migration> MigrationsToRevert { get; set; } = [];
    }
}
