using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jaryway.DynamicSpace.DynamicWebApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DynamicClassBase
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public long Id { get; set; }
    }
}
