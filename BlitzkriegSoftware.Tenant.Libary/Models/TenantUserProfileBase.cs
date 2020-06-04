using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace BlitzkriegSoftware.Tenant.Libary.Models
{
    /// <summary>
    /// Base class that implements Tenant User Profile
    /// </summary>
    public class TenantUserProfileBase : ITenantUserProfile
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        [Required]
        public Guid _id { get; set; }

        /// <summary>
        /// Unique Identifier from AuthN
        /// </summary>
        public string UniqueUserId { get; set; }

        private List<Guid> _tenants;

        /// <summary>
        /// List of tenants, should never be null, or empty
        /// </summary>
        public List<Guid> Tenants
        {
            get
            {
                if (this._tenants == null)
                {
                    this._tenants = new List<Guid>();
                }
                return this._tenants;
            }
            set
            {
                this._tenants = value;
            }
        }

        private Dictionary<string, object> _settings;

        /// <summary>
        /// (optional) additional settings (including user info obtained from AuthN)
        /// </summary>
        public Dictionary<string, object> Settings
        {
            get
            {
                if (this._settings == null)
                {
                    this._settings = new Dictionary<string, object>();
                }
                return this._settings;
            }
            set
            {
                this._settings = value;
            }
        }

        /// <summary>
        /// Checks the information on this instance to make sure it is valid
        /// </summary>
        public bool UserIsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.UniqueUserId) &&
                       (this.Tenants != null) &&
                       (this.Tenants.Count() > 0) &&
                       (this.Settings != null) &&
                       (this.Settings.Count() > 0)
                       ;

            }
        }

        /// <summary>
        /// Passed a name,value pair this attempts to add/update settings
        /// <para>Mapping the name and transforming the value if required</para>
        /// </summary>
        /// <param name="key">(sic)</param>
        /// <param name="value">(sic)</param>
        /// <returns>True if updated, False if added</returns>
        public bool SettingsPut(string key, string value)
        {
            bool isChanged = false;

            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            if(this.Settings.ContainsKey(key))
            {
                this.Settings[key] = value;
                isChanged = true;
            } else
            {
                this.Settings.Add(key, value);
            }
            
            return isChanged;
        }

        /// <summary>
        /// Debugging String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Id: {this._id}, Username: {this.UniqueUserId}, Tenants: {string.Join(",", this.Tenants)}, Settings: {string.Join(";", this.Settings.Select(x => string.Join("=", x.Key, x.Value)))}";
        }

        /// <summary>
        /// Test Equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is TenantUserProfileBase)
            {
                var m = obj as TenantUserProfileBase;
                var flag = 
                    this._id.Equals(m._id) &&
                    this.UniqueUserId.Equals(m.UniqueUserId) 
                    ;
                foreach(var t in m.Tenants)
                {
                    if(!this.Tenants.Contains(t)) {
                        flag = false;
                        break;
                    }
                }
                return flag;
            }
            return false;
        }

    }
}
