using System;
using System.Collections.Generic;
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
        /// 迁移名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 文件内容
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 是否是Snapshot文件
        /// </summary>
        public SourceCodeKind SourceCodeKind { get; set; }
    }
}
