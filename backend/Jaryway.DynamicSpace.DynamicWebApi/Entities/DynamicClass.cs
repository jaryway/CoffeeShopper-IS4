using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Jaryway.DynamicSpace.DynamicWebApi.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [Table("DynamicClasses")]
    public partial class DynamicClass
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
        /// <summary>
        /// 设计时
        /// </summary>
        public string EntityProperties_ { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public bool EntityPropertiesHasChanged
        {
            get
            {
                var designTimeProperties = this.GetDesignTimeProperties();
                var s1 = Regex.Replace(designTimeProperties, @"get;", " get; ");
                s1 = Regex.Replace(s1, " set; ", " set; ");
                s1 = Regex.Replace(s1, @"\s+", " ");
                var s2 = Regex.Replace(EntityProperties, @"get;", " get; ");
                s2 = Regex.Replace(s2, " set; ", " set; ");
                s2 = Regex.Replace(s2, @"\s+", " ");

                return s1 != s2;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [DefaultValue("[{name:\"Name\",\"dataType\":0}]")]
        public string JSON { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public bool Published { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        public int? ProjectId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(64)]
        public string? TenantId { get; set; }

    }
}
