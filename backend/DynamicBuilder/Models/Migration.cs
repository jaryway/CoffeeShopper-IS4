using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBuilder.Models
{
    [Table("MigrationEntries")]
    public class MigrationEntry
    {
        [Key]
        [Required]
        public string MigrationId { get; set; }
        public string Code { get; set; } = string.Empty;
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
