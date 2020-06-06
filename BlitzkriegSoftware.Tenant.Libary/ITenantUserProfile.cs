using System;
using System.Collections.Generic;

namespace BlitzkriegSoftware.Tenant
{
    /// <summary>
    /// Define the minimum data contract for a User Profile
    /// <para>These are shared across tenants</para>
    /// </summary>
    public interface ITenantUserProfile
    {
        /// <summary>
        /// Key
        /// </summary>
        Guid _id { get; set; }

        /// <summary>
        /// Unique Identifier form AuthN
        /// </summary>
        string UniqueUserId { get; set; }
        
        /// <summary>
        /// List of tenants, should never be null, or empty
        /// </summary>
        List<Guid> Tenants { get; set; }

        /// <summary>
        /// (optional) additional settings (including user info obtained from AuthN)
        /// </summary>
        Dictionary<string, object> Settings { get; set; }

        /// <summary>
        /// Checks the information on this instance to make sure it is valid
        /// </summary>
        bool UserIsValid { get; }

        /// <summary>
        /// Passed a name,value pair this attempts to add/update settings
        /// <para>Mapping the name and transforming the value if required</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool SettingsPut(string key, string value);
    }
}
