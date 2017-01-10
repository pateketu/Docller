using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docller.Core.Models
{
    public class Transmittal:BaseFederatedModel
    {
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

        public long TransmittalId { get; set; }
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string TransmittalNumber { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsDraft { get; set; }
        public User CreatedBy { get; set; }
        public List<TransmittalUser> Distribution { get; set; }
        public List<TransmittedFile> Files { get; set; }
        public Status TransmittalStatus { get; set; }

        public string BlobContainer { get; set; }
    }
}
