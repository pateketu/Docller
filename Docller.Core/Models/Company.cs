using System;
using System.Collections.Generic;
using Docller.Core.Common;
using Newtonsoft.Json;

namespace Docller.Core.Models
{
    public class Company:BaseModel
    {
        public Company()
        {
            Users = new List<User>();
        }
        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        /// <value>
        /// The company id.
        /// </value>
        public long CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the name of the company.
        /// </summary>
        /// <value>
        /// The name of the company.
        /// </value>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the company alias.
        /// </summary>
        /// <value>
        /// The company alias.
        /// </value>
        [JsonIgnore]
        public string CompanyAlias { get; set; }


        /// <summary>
        /// Gets or sets the revision reg ex.
        /// </summary>
        /// <value>
        /// The revision reg ex.
        /// </value>
        [JsonIgnore]
        public string RevisionRegEx
        {
            get;
            set;
        }

        [JsonIgnore]
        public string AttributeMappingsXml { get; set; }

        public List<User> Users { get; set; }
        public PermissionFlag Permission { get; set; }
        
        /// <summary>
        /// Stroed Procedure to ExecuteNonQuery single entity
        /// </summary>
        internal override string InsertProc
        {
            get { return "usp_AddCompany"; }
        }

        /// <summary>
        /// Stroed Procedure to Update single entity
        /// </summary>
        internal override string UpdateProc
        {
            get { throw new System.NotImplementedException(); }
        }

        /// <summary>
        /// Stroed Procedure to delete single entity
        /// </summary>
        internal override string DeleteProc
        {
            get { throw new System.NotImplementedException(); }
        }

        /// <summary>
        /// Stored Procedure to Populate the Single Entity
        /// </summary>
        internal override string GetProc
        {
            get { throw new System.NotImplementedException(); }
        }

    }
}