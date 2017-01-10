using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;
using Docller.Core.Common;
using Docller.Core.Repository;


namespace Docller.Core.Models
{
    public class Project:BaseFederatedModel,ISecureable
    {
        public long ProjectId { get; set; }

        [DataType(DataType.Text)]
        [Required()]
        public string ProjectName { get; set; }

        [DataType(DataType.Text)]
        public string ProjectCode { get; set; }

        [DataType(DataType.ImageUrl)]
        [System.ComponentModel.DataAnnotations.FileExtensions]
        public string ProjectImage { get; set; }
        
        public string BlobContainer { get; set; }
        public PermissionFlag CurrentUserPermissions { get; set; }
        /// <summary>
        /// Stroed Procedure to ExecuteNonQuery single entity
        /// </summary>
        internal override string InsertProc
        {
            get { return null; }
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
            get { return StoredProcs.GetProjectDetails; }
        }

       
    }
}
