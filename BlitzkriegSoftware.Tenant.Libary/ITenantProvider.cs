using System;
using System.Collections.Generic;
using BlitzkriegSoftware.Tenant.Libary.Models;

namespace BlitzkriegSoftware.Tenant.Libary
{
    /// <summary>
    /// Generic Contract: Tenancy Providers
    /// </summary>
    public interface ITenantProvider
    {
        /// <summary>
        /// Gets back the detailed tenant contact information
        /// </summary>
        /// <param name="tenantId">Primary Key</param>
        /// <returns>ITenantContact</returns>
        ITenantContact ContactGet(Guid tenantId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId">Primary Key</param>
        /// <param name="contact"></param>
        /// <returns></returns>
        void ContactUpdate(Guid tenantId, ITenantContact contact);

        /// <summary>
        /// Fetch Configuration
        /// </summary>
        /// <param name="tenantId">Primary Key</param>
        /// <param name="keys">List of keys to return (null is all config)</param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> ConfigurationGet(Guid tenantId, IEnumerable<string> keys = null);

        /// <summary>
        /// Fetch Configuration
        /// </summary>
        /// <param name="tenantId">Primary Key</param>
        /// <param name="startsWith">Filter down to keys that `start with` (empty is all config)</param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> ConfigurationGet(Guid tenantId, string startsWith = "");

        /// <summary>
        /// Adds/Updates configuration for a tenant
        /// </summary>
        /// <param name="tenantId">Primary Key</param>
        /// <param name="config">Configuration as used by <c>Microsoft.Extensions.Configuration</c> <c>MemoryCollection</c></param>
        /// <returns></returns>
        void ConfigurationUpdate(Guid tenantId, IEnumerable<KeyValuePair<string, string>> config);

        /// <summary>
        /// Does Tenant Exist
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        bool TenantExists(Guid tenantId);

        /// <summary>
        /// Add/Update an entire tenant graph
        /// </summary>
        /// <param name="model">ITenantModel</param>
        /// <returns>True if created, False if updated</returns>
        bool TenantAddUpdate(ITenantModel model);

        /// <summary>
        /// Get entire tenant
        /// </summary>
        /// <param name="tenantId">Id</param>
        /// <returns>Tenant Model</returns>
        ITenantModel TenantGet(Guid tenantId);

        /// <summary>
        /// Delete Tenant
        /// <para>Implementationn should call out if this is destructive</para>
        /// </summary>
        /// <param name="tenantId">Primary Key</param>
        /// <returns>True if deleted</returns>
        bool TenantDelete(Guid tenantId);
    }
}
