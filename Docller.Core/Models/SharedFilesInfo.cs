using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docller.Core.Models
{
    public class SharedFilesInfo
    {
        public long TransmittalId { get; set; }
        public IEnumerable<File> Files { get; set; } 
    }
}
