using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using DataAnnotationsExtensions;
using Docller.Core.Common;
using Newtonsoft.Json;


namespace Docller.Core.Models
{
    public class User:BaseModel
    {
        private Company _company;

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        [JsonIgnore]
        public int UserId
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [JsonIgnore]
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [DataType(DataType.EmailAddress)]
        [Required]
        [Email]
        public string Email { get; set; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.LastName) || !string.IsNullOrEmpty(LastName))
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0} {1}", this.FirstName, this.LastName).Trim();
                }
                return this.Email != null ? this.Email.Trim() : string.Empty;
            }
        }


        /// <summary>
        /// Gets or sets the temp password.
        /// </summary>
        /// <value>
        /// The temp password.
        /// </value>
        [JsonIgnore]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the password salt.
        /// </summary>
        /// <value>
        /// The password salt.
        /// </value>
        [JsonIgnore]
        public string PasswordSalt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is locked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is locked; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [force password change].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [force password change]; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool ForcePasswordChange { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        /// <value>
        /// The company.
        /// </value>
        [JsonIgnore]
        public Company Company
        {
            get { return this._company ?? (this._company = new Company()); }

            set { this._company = value; }

        }
        [JsonIgnore]
        public List<Project> Projects { get; set; }
        [JsonIgnore]
        public bool IsCustomerAdmin { get; set; }

        [JsonIgnore]
        public PermissionFlag CustomerPermissions { get; set; }

        [JsonIgnore]
        public bool HasMultipleProjects { get; set; }

        [JsonIgnore]
        public int FailedLogInAttempt { get; set; }

        /// <summary>
        /// Stroed Procedure to ExecuteNonQuery single entity
        /// </summary>
        internal override string InsertProc
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Stroed Procedure to Update single entity
        /// </summary>
        internal override string UpdateProc
        {
            get { return "usp_UpdateUser"; }
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
            get { return "usp_GetUserForLogOn"; }
        }

        //public bool Cced { get; set; }
        [JsonIgnore]
        public bool IsNew { get; set; }

    }
}
