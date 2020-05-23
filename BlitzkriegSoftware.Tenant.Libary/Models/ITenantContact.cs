using System;
using System.ComponentModel.DataAnnotations;

namespace BlitzkriegSoftware.Tenant.Libary.Models
{

    /// <summary>
    /// Tenant Contract
    /// </summary>
    public interface ITenantContact
    {

        /// <summary>
        /// Tenant Display Name
        /// </summary>
        [Required]
        [StringLength(256, MinimumLength = 1)]
        string DisplayName { get; set; }

        /// <summary>
        /// Phone (optional)
        /// </summary>
        [Phone]
        string ContactPhone { get; set; }

        /// <summary>
        /// EMail of Primary Contact
        /// </summary>
        [EmailAddress]
        string ContactEmail { get; set; }

        /// <summary>
        /// Contact Name
        /// </summary>
        [StringLength(256)]
        string ContactName { get; set; }
    }

}
