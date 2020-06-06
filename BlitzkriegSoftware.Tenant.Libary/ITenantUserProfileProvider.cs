using System;

namespace BlitzkriegSoftware.Tenant
{
    /// <summary>
    /// UserProfile User Profile Provider
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITenantUserProfileProvider<T> where T : ITenantUserProfile , new()
    {

        /// <summary>
        /// Read a UserProfile
        /// </summary>
        /// <param name="id">Key</param>
        /// <returns>UserProfile of Type T or Null</returns>
        T Read(string id);

        /// <summary>
        /// Write UserProfile
        /// </summary>
        /// <param name="model">UserProfile of Type T</param>
        void Write(T model);

        /// <summary>
        /// Does UserProfile Exist?
        /// </summary>
        /// <param name="id">Key</param>
        /// <returns>True if so</returns>
        bool Exists(string id);

        /// <summary>
        /// Delete a UserProfile
        /// <para>Warning Destructive</para>
        /// </summary>
        /// <param name="id">Key</param>
        /// <returns>True if so</returns>
        bool Delete(string id);

    }
}
