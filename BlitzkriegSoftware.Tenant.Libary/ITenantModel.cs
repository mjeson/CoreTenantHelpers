using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlitzkriegSoftware.Tenant
{
    /// <summary>
    /// Tenant Model Interface
    /// </summary>
    public interface ITenantModel
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        [Required]
        Guid _id { get; set; }

        /// <summary>
        /// Contact
        /// </summary>
        ITenantContact Contact { get; set; }

        /// <summary>
        /// Configuration
        /// </summary>
        IEnumerable<KeyValuePair<string, string>> Configuration { get; set; }
    }
}
