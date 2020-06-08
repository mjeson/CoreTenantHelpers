using BlitzkriegSoftware.Tenant.Libary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace BlitzkriegSoftware.Tenant.FileProvider
{
    /// <summary>
    /// Tenant Data Provider: File
    /// </summary>
    public class FileTenantDataProvider<T>: ITenantDataProvider<T> where T : ITenantModel, new()
    {

        #region "Privates, etc."
        private readonly DirectoryInfo _rootFolder;
        #endregion

        #region "CTOR"

        [ExcludeFromCodeCoverage]
        private FileTenantDataProvider() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="rootFolder">Folder where tenants are stored</param>
        public FileTenantDataProvider(string rootFolder)
        {
            this._rootFolder = new DirectoryInfo(rootFolder);
        }

        #endregion

        #region "Read/Write"

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="id">Key</param>
        /// <returns>Tenant of Type T</returns>
        [ExcludeFromCodeCoverage]
        public T Read(Guid id)
        {
            var model = default(T);

            if (this._rootFolder.Exists)
            {
                var filename = Path.Combine(this._rootFolder.FullName, $"{id}.json");
                if (File.Exists(filename))
                {
                    var json = File.ReadAllText(filename);
                    model = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
                    {
                        // Ignore Interface
#pragma warning disable SCS0028 // TypeNameHandling is set to other value than 'None' that may lead to deserialization vulnerability
                        TypeNameHandling = TypeNameHandling.Objects
#pragma warning restore SCS0028 // TypeNameHandling is set to other value than 'None' that may lead to deserialization vulnerability
                    });
                }
            }
            return model;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="model">Tenant of Type T</param>
        [ExcludeFromCodeCoverage]
        public void Write(T model)
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

            if (model._id == Guid.Empty)
            {
                throw new ArgumentException($"A model must contain a `TenantId`", nameof(model));
            }

            if (this._rootFolder.Exists)
            {
                var filename = Path.Combine(this._rootFolder.FullName, $"{model._id}.json");
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                string json = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
                {
                    // Ignore Interface
#pragma warning disable SCS0028 // TypeNameHandling is set to other value than 'None' that may lead to deserialization vulnerability
                    TypeNameHandling = TypeNameHandling.Objects
#pragma warning restore SCS0028 // TypeNameHandling is set to other value than 'None' that may lead to deserialization vulnerability
                });

                File.WriteAllText(filename, json);
            }
        }

        /// <summary>
        /// Exists
        /// </summary>
        /// <param name="tenantId">Key</param>
        /// <returns>True if so</returns>
        [ExcludeFromCodeCoverage]
        public bool Exists(Guid tenantId)
        {
            if (this._rootFolder.Exists)
            {
                var filename = Path.Combine(this._rootFolder.FullName, $"{tenantId}.json");
                return (File.Exists(filename));
            }
            return false;
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="tenantId">Key</param>
        /// <returns>True if so</returns>
        [ExcludeFromCodeCoverage]
        public bool Delete(Guid tenantId)
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

        #endregion

    }
}
