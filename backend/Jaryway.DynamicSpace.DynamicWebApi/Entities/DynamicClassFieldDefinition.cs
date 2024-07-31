namespace Jaryway.DynamicSpace.DynamicWebApi.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicClassFieldDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public DynamicClassFieldDataType DataType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Reference { get; set; } = string.Empty;
        /// <summary>
        /// 默认值
        /// </summary>
        public string? DefaultValue { get; set; }
        /// <summary>
        /// 唯一值
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// 非空
        /// </summary>
        public bool NotNull { get; set; }


    }
}
