using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Models;
using Docller.Core.Repository;

namespace Docller.Core.Infrastructure
{
    public abstract class SProcOutPutInfo
    {
        public int ReturnVal { get; set; }
        public RepositoryStatus Status { get; set; }
    }

    public class CustomerSubInfo:SProcOutPutInfo
    {
        public bool IsExistingUser { get; set; }
    }

    public class FilesInfo:SProcOutPutInfo
    {
        public IEnumerable<File> DuplicateFiles { get; set; }
    }

    public class CreateTransmittalInfo:SProcOutPutInfo
    {
        public IssueSheet IssueSheetData { get; set; }
        public Transmittal Transmittal { get; set; }
    }
}
