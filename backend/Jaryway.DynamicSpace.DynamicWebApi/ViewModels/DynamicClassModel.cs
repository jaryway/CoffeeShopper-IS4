using System.ComponentModel.DataAnnotations;

namespace Jaryway.DynamicSpace.DynamicWebApi.ViewModels;

/// <summary>
/// 
/// </summary>
public class DynamicClassModel
{
    /// <summary>
    /// 
    /// </summary>
    public int? Id { get; set; } = null;
    /// <summary>
    /// 
    /// </summary>
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
    /// 属性字段 EntityProperties 和 JSON 必填一个
    /// </summary>
    // [Required]
    public string EntityProperties { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string JSON { get; set; } = string.Empty;
}

