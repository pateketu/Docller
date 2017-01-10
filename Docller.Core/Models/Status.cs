using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docller.Core.Models
{
    public class Status:BaseFederatedModel
    {
        public long StatusId { get; set; }
        public long ProjectId { get; set; }
        public string StatusText { get; set; }
        public string StatusLongText { get; set; }
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
