using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docller.Core.Models
{
    public class IssueSheet
    {
        public string TransmittalNumber { get; set; }
        public long TransmittalId { get; set; }
        public string ProjectName { get; set; }   
        public string CustomerLogo { get; set; }
        public List<Status> AllStatus {get;set;}
        public List<TransmittedFile> IssuedFiles { get; set; }
        public List<TransmittalUser> Distribution { get; set; }
        public List<TransmittalInfo> Transmittals { get; set; }
        
        public string BlobContainer { get; set; }
    }
}
