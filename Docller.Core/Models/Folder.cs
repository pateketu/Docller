using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Common;
using Newtonsoft.Json;

namespace Docller.Core.Models
{
    public class Folder : BaseFederatedModel, ISecureable
    {
        public long FolderId { get; set; }

        [JsonIgnore]
        public long ParentFolderId { get; set; }

        [JsonIgnore]
        public long ProjectId { get; set; }

        [JsonProperty("title")]
        public string FolderName { get; set; }

        [JsonIgnore]
        public string FolderInternalName { get; set; }

        public string FullPath { get; set; }

        [JsonProperty("children")]
        public List<Folder> SubFolders { get; set; }

        [JsonIgnore]
        public List<long> AllParents { get; set; }


        [JsonIgnore]
        public PermissionFlag CurrentUserPermissions { get; set; }

        [JsonIgnore]
        internal override string InsertProc
        {
            get { return "usp_AddFolders"; }
        }

        [JsonIgnore]
        internal override string UpdateProc
        {
            get { throw new NotImplementedException(); }
        }

        [JsonIgnore]
        internal override string DeleteProc
        {
            get { throw new NotImplementedException(); }
        }

        [JsonIgnore]
        internal override string GetProc
        {
            get { throw new NotImplementedException(); }
        }
    }
}
