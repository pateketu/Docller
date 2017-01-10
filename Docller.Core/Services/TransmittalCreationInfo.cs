using System.Collections.Generic;
using Docller.Core.Models;

namespace Docller.Core.Services
{
    public class TransmittalCreationInfo
    {
        public IssueSheet IssueSheetData { get; set; }

        public TransmittalServiceStatus Status { get; set; }

        public Transmittal Transmittal { get; set; }

        
    }
}