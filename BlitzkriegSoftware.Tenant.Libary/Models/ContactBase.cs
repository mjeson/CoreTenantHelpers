using System;

namespace BlitzkriegSoftware.Tenant.Libary.Models
{
    /// <summary>
    /// Model: Contact
    /// </summary>
    public class ContactBase : ITenantContact
    {
        /// <summary>
        /// Key
        /// </summary>
        public Guid TenantId { get; set; }
        /// <summary>
        /// Display Name
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Contact Phone
        /// </summary>
        public string ContactPhone { get; set; }
        /// <summary>
        /// Contract E-Mail
        /// </summary>
        public string ContactEmail { get; set; }
        /// <summary>
        /// Contact Name
        /// </summary>
        public string ContactName { get; set; }
    }
}
