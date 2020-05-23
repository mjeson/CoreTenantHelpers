using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlitzkriegSoftware.Tenant.Libary.Models
{
    /// <summary>
    /// Model: Tenant Base
    /// </summary>
    public class TenantBase : ITenantModel
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        [Required]
        public Guid _id { get; set; }

        /// <summary>
        /// Contact
        /// </summary>
        public ITenantContact Contact { get; set; }

        /// <summary>
        /// Configuration
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Configuration { get; set; }
    }
}
