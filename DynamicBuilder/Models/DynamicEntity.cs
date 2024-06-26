using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBuilder.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicEntity : EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public override string TableName => "DynamicEntities";

        /// <summary>
        /// 属性代码
        /// <code>
        /// "public string Name{get;set;} public int Age{get;set;} "
        /// </code>
        /// </summary>

        public string EntityProperties { get; set; } = string.Empty;
    }
}
