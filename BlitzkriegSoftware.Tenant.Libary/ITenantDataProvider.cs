using BlitzkriegSoftware.Tenant.Libary.Models;
using System;

namespace BlitzkriegSoftware.Tenant.Libary
{
    /// <summary>
    /// Tenant Data Provider
    /// </summary>
    public interface ITenantDataProvider<T> where T : ITenantModel, new()
    {
        /// <summary>
        /// Read a tenant
        /// </summary>
        /// <param name="id">Key</param>
        /// <returns>Tenant of Type T or Null</returns>
        T Read(Guid id);

        /// <summary>
        /// Write tenant
        /// </summary>
        /// <param name="model">Tenant of Type T</param>
        void Write(T model);

        /// <summary>
        /// Does Tenant Exist?
        /// </summary>
        /// <param name="id">Key</param>
        /// <returns>True if so</returns>
        bool Exists(Guid id);

        /// <summary>
        /// Delete a tenant
        /// <para>Warning Destructive</para>
        /// </summary>
        /// <param name="id">Key</param>
        /// <returns>True if so</returns>
        bool Delete(Guid id);

    }
}
