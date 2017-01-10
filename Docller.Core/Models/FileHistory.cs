using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docller.Core.Models
{
    public class FileHistory
    {
        public File File { get; set; }
        public IEnumerable<Transmittal> Transmittals { get; set; } 
        
    }
}
