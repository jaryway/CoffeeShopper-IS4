using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSpace.Models
{
    [Table("DynamicEntities")]
    public class DynamicEntity
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        //public string Namespace { get; set; } = string.Empty;
        /// <summary>
        /// 数据库表名称
        /// </summary>
        public string TableName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// 属性字段
        /// </summary>
        public string EntityProperties { get; set; } = string.Empty;
        public string JSON { get; set; } = string.Empty;

        public bool Published { get; set; } = false;

        public int? ProjectId { get; set; }

        [MaxLength(64)]
        public string? TenantId { get; set; }
    }
}
