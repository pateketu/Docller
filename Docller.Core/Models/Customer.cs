using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Docller.Core.MVCExtensions;
using Docller.Core.Repository;

namespace Docller.Core.Models
{
    public class Customer: BaseFederatedModel
    {

        /// <summary>
        /// Gets or sets the name of the customer.
        /// </summary>
        /// <value>The name of the customer.</value>
        /// <remarks></remarks>
        [DataType(DataType.Text)]
        [Required()]
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets the admin user.
        /// </summary>
        /// <value>
        /// The admin user.
        /// </value>
        public User AdminUser { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is trial.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is trial; otherwise, <c>false</c>.
        /// </value>
        public bool IsTrial { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the domain URL.
        /// </summary>
        /// <value>
        /// The domain URL.
        /// </value>
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Sud domain is Required")]
        [StringLength(maximumLength: 10, MinimumLength = 3, ErrorMessage = "Sub Domain should be Minimum 3 characters and Maximum 10 characters")]
        [Remote("IsDomainUrlInUse","Customer",ErrorMessage = "Sub Domain is in use")]
        [NoSpace("Space is not allowed in Domain Name")]
        public string DomainUrl { get; set; }

        /// <summary>
        /// Stroed Procedure to ExecuteNonQuery single entity
        /// </summary>
        internal override string InsertProc
        {
            get { return "usp_AddCustomer"; }
        }

        /// <summary>
        /// Stroed Procedure to Update single entity
        /// </summary>
        internal override string UpdateProc
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Stroed Procedure to delete single entity
        /// </summary>
        internal override string DeleteProc
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Stored Procedure to Populate the Single Entity
        /// </summary>
        internal override string GetProc
        {
            get { return StoredProcs.GetCutomer; }
        }
    }
}
