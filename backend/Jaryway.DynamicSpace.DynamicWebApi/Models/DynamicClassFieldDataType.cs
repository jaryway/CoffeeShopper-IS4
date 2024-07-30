using System.ComponentModel.DataAnnotations;

namespace Jaryway.DynamicSpace.DynamicWebApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public enum DynamicClassFieldDataType
    {
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "string")]
        TEXT = 0,
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "int")]
        INTEGER = 1,
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "decimel")]
        NUMERIC = 2,
    }
}
