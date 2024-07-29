using System.ComponentModel.DataAnnotations;

namespace Jaryway.DynamicSpace.DynamicWebApi.Models
{
    public enum DynamicClassFieldDataType
    {
        [Display(Name = "string")]
        TEXT = 0,
        [Display(Name = "int")]
        INTEGER = 1,
        [Display(Name = "decimel")]
        NUMERIC = 2,
    }
}
