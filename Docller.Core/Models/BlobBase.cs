using System;

namespace Docller.Core.Models
{
    public class BlobBase : BaseFederatedModel
    {
        public string UniqueIdentifier { get; set; }
        public long FileId { get; set; }
        public Guid FileInternalName { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public decimal FileSize { get; set; }
        public Folder Folder { get; set; }
        public Project Project { get; set; }
        public User CreatedBy { get; set; }
        public User ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        
        
        #region Overrides of BaseModel

        internal override string InsertProc
        {
            get { throw new NotImplementedException(); }
        }

        internal override string UpdateProc
        {
            get { throw new NotImplementedException(); }
        }

        internal override string DeleteProc
        {
            get { throw new NotImplementedException(); }
        }

        internal override string GetProc
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

    
}