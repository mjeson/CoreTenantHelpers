using System.Collections.Generic;

namespace BlitzkriegSoftware.Tenant.Libary.Models
{
    /// <summary>
    /// Model: Tenant Base
    /// </summary>
    public class TenantBase : ITenantModel
    {
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
