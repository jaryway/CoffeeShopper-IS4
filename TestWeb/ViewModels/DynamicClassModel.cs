using System.ComponentModel.DataAnnotations;

namespace TestWeb.ViewModels
{
    public class DynamicClassModel
    {
        public int? Id { get; set; } = null;

        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 数据库表名称
        /// </summary>
        [Required]
        public string TableName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string? Description { get; set; } = string.Empty;
        /// <summary>
        /// 属性字段
        /// </summary>
        [Required]
        public string EntityProperties { get; set; } = string.Empty;
    }
}
