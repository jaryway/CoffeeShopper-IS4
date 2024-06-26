using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBuilder.Models
{

    public enum SourceCodeKind
    {
        Unknown = 0,
        Base = 1,
        Entity = 2,
        Snapshot = 3,
        Migration = 4,
        MigrationMetadata = 5,
    }
    public class SourceCode
    {
        public int Id { get; set; }
        /// <summary>
        /// 名称 eg. User.cs
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 文件内容
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 是否是Snapshot文件
        /// </summary>
        public SourceCodeKind SourceCodeKind { get; set; }

        /// <summary>
        /// 仅当 SourceCodeKind = Entity 时有效
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        public bool Published { get; set; } = false;

        public int? ProjectId { get; set; }

        [MaxLength(64)]
        public string? TenantId { get; set; }
    }
}
