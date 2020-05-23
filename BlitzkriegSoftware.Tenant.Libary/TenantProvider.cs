using BlitzkriegSoftware.Tenant.Libary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlitzkriegSoftware.Tenant.Libary
{
    /// <summary>
    /// Tenant Provider
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TenantProvider<T> where T : ITenantModel, new()
    {
        #region "Constants"
        private readonly ITenantDataProvider<T> _dataProvider;
        #endregion

        #region "CTOR"
        private TenantProvider() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="dataProvider"></param>
        public TenantProvider(ITenantDataProvider<T> dataProvider)
        {
            this._dataProvider = dataProvider;
        }
        #endregion

        #region "ITenantProvider"

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

            var model = this._dataProvider.Read(tenantId);

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
        public void ContactUpdate(Guid tenantId, ITenantContact contact)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("Tenant Must be Supplied", nameof(tenantId));
            }

            var model = this._dataProvider.Read(tenantId);
            if (model == null)
            {
                throw new InvalidOperationException($"No tenant found for {tenantId}");
            }
            model.Contact = contact;
            this._dataProvider.Write(model);
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

            var model = this._dataProvider.Read(tenantId);
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

            var model = this._dataProvider.Read(tenantId);
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
        public void ConfigurationUpdate(Guid tenantId, IEnumerable<KeyValuePair<string, string>> config)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("Tenant Must be Supplied", nameof(tenantId));
            }

            if (config == null)
            {
#pragma warning disable IDE0016 // Use 'throw' expression
                throw new ArgumentNullException(nameof(config));
#pragma warning restore IDE0016 // Use 'throw' expression
            }

            var model = this._dataProvider.Read(tenantId);

            if (model == null)
            {
                throw new InvalidOperationException($"Tenant not found for {tenantId}");
            }

            model.Configuration = config;
            this._dataProvider.Write(model);
        }

        /// <summary>
        /// Tenant Exists?
        /// </summary>
        /// <param name="tenantId">Key</param>
        /// <returns>True if exists</returns>
        public bool TenantExists(Guid tenantId)
        {
            return this._dataProvider.Exists(tenantId);
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

            if (model._id == Guid.Empty)
            {
                throw new ArgumentException($"A model must contain a `TenantId`", nameof(model));
            }

            bool exists = this.TenantExists(model._id);
            this._dataProvider.Write((T)model);
            return exists;
        }

        /// <summary>
        /// Tenant Get
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>Tenant Model</returns>
        public ITenantModel TenantGet(Guid tenantId)
        {
            return this._dataProvider.Read(tenantId);
        }

        /// <summary>
        /// Delete tenant
        /// <para>Warning! Destructive!</para>
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>True is delete success</returns>
        public bool TenantDelete(Guid tenantId)
        {
            return this._dataProvider.Delete(tenantId);
        }

        #endregion

    }
}
