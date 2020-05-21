using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using BlitzkriegSoftware.Tenant.Libary;
using BlitzkriegSoftware.Tenant.Libary.Models;
using Newtonsoft.Json;

namespace BlitzkriegSoftware.Tenant.FileProvider
{
    /// <summary>
    /// Tenant Provider: Json Files
    /// </summary>
    public class JsonFileProvider : ITenantProvider
    {
        private readonly DirectoryInfo _rootFolder;

        [ExcludeFromCodeCoverage]
        private JsonFileProvider() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="rootFolder"></param>
        public JsonFileProvider(DirectoryInfo rootFolder)
        {
            this._rootFolder = rootFolder;
        }

        /// <summary>
        /// Contact Get
        /// </summary>
        /// <param name="tenantId">Key</param>
        /// <returns>Contact</returns>
        public ITenantContact ContactGet(Guid tenantId)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("Tenant Must be Supplied", nameof(tenantId));
            }

            var model = this.Read(tenantId);

            if (model == null)
            {
                throw new InvalidOperationException($"Contact Not Found for {tenantId}");
            }

            return model.Contact;
        }

        /// <summary>
        /// Contact Add/Update
        /// </summary>
        /// <param name="tenantId">Key</param>
        /// <param name="contact">Contact</param>
        /// <returns>True if existed, false if new</returns>
        public void ContactUpdate(Guid tenantId, ITenantContact contact)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("Tenant Must be Supplied", nameof(tenantId));
            }

            var model = this.Read(tenantId);
            if (model == null)
            {
                throw new InvalidOperationException($"No tenant found for {tenantId}");
            }
            model.Contact = contact;
            this.Write(model);
        }

        /// <summary>
        /// Configuration Get
        /// </summary>
        /// <param name="tenantId">Key</param>
        /// <param name="keys">(optional) Keys to fetch</param>
        /// <returns>Configuration</returns>
        public IEnumerable<KeyValuePair<string, string>> ConfigurationGet(Guid tenantId, IEnumerable<string> keys = null)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("Tenant Must be Supplied", nameof(tenantId));
            }

            var model = this.Read(tenantId);
            if (model == null)
            {
                throw new InvalidOperationException($"Config Not Found for {tenantId}");
            }

            if ((keys == null) || (keys.Count() <= 0))
            {
                return model.Configuration;
            }
            else
            {
                return model.Configuration.Where(k => keys.Contains(k.Key));
            }
        }

        /// <summary>
        /// Configuration Get
        /// </summary>
        /// <param name="tenantId">Key</param>
        /// <param name="startsWith">(optional) starts with</param>
        /// <returns>Configuration</returns>
        public IEnumerable<KeyValuePair<string, string>> ConfigurationGet(Guid tenantId, string startsWith = "")
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("Tenant Must be Supplied", nameof(tenantId));
            }

            var model = this.Read(tenantId);
            if (model == null)
            {
                throw new InvalidOperationException($"Config Not Found for {tenantId}");
            }

            if (string.IsNullOrEmpty(startsWith))
            {
                return model.Configuration;
            }
            else
            {
                return model.Configuration.Where(k => k.Key.StartsWith(startsWith));
            }
        }

        /// <summary>
        /// ConfigurationAddUpdate
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="config">Configuration is entirely replaced</param>
        /// <returns>True if existed</returns>
        public void ConfigurationUpdate(Guid tenantId, IEnumerable<KeyValuePair<string, string>> config)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("Tenant Must be Supplied", nameof(tenantId));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var model = this.Read(tenantId);

            if (model == null)
            {
                throw new InvalidOperationException($"Tenant not found for {tenantId}");
            }

            model.Configuration = config;
            this.Write(model);
        }

        /// <summary>
        /// Tenant Exists?
        /// </summary>
        /// <param name="tenantId">Key</param>
        /// <returns>True if exists</returns>
        public bool TenantExists(Guid tenantId)
        {
            if (this._rootFolder.Exists)
            {
                var filename = Path.Combine(this._rootFolder.FullName, $"{tenantId}.json");
                return (File.Exists(filename));
            }
            return false;
        }

        /// <summary>
        /// Tenant Add/Update
        /// </summary>
        /// <param name="model"></param>
        /// <returns>True if created, false if updated</returns>
        public bool TenantAddUpdate(ITenantModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Configuration == null)
            {
                model.Configuration = new List<KeyValuePair<string, string>>();
            }

            if (model.Contact == null)
            {
                model.Contact = new ContactBase();
            }

            if (model.Contact.TenantId == Guid.Empty)
            {
                throw new ArgumentException($"A model must contain a `TenantId`", nameof(model));
            }

            var exists = this.TenantExists(model.Contact.TenantId);
            this.Write((TenantBase)model);
            return exists;
        }

        /// <summary>
        /// Tenant Get
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>Tenant Model</returns>
        public ITenantModel TenantGet(Guid tenantId)
        {
            return this.Read(tenantId);
        }

        /// <summary>
        /// Delete tenant
        /// <para>Warning! Destructive!</para>
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public bool TenantDelete(Guid tenantId)
        {
            if (this._rootFolder.Exists)
            {
                var filename = Path.Combine(this._rootFolder.FullName, $"{tenantId}.json");
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                    return true;
                }
            }
            return false;
        }

        [ExcludeFromCodeCoverage]
        private TenantBase Read(Guid tenantId)
        {
            TenantBase model = null;

            if (this._rootFolder.Exists)
            {
                var filename = Path.Combine(this._rootFolder.FullName, $"{tenantId}.json");
                if (File.Exists(filename))
                {
                    var json = File.ReadAllText(filename);
                    model = JsonConvert.DeserializeObject<TenantBase>(json, new JsonSerializerSettings
                    {
                        // Ignore Interface
                        TypeNameHandling = TypeNameHandling.Objects
                    });
                }
            }
            return model;
        }

        [ExcludeFromCodeCoverage]
        private void Write(TenantBase model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Configuration == null)
            {
                model.Configuration = new List<KeyValuePair<string, string>>();
            }

            if (model.Contact == null)
            {
                model.Contact = new ContactBase();
            }

            if (model.Contact.TenantId == Guid.Empty)
            {
                throw new ArgumentException($"A model must contain a `TenantId`", nameof(model));
            }

            if (this._rootFolder.Exists)
            {
                var filename = Path.Combine(this._rootFolder.FullName, $"{model.Contact.TenantId}.json");
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                string json = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
                {
                    // Ignore Interface
                    TypeNameHandling = TypeNameHandling.Objects
                });

                File.WriteAllText(filename, json);
            }
        }

    }
}
