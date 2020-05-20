using System.Collections.Generic;

namespace BlitzkriegSoftware.Tenant.Libary.Models
{
    /// <summary>
    /// Tenant Model Interface
    /// </summary>
    public interface ITenantModel
    {
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
