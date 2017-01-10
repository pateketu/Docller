using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docller.Core.Models
{
    public class TransmittalEmailView
    {
        
        public TransmittalEmailView(Transmittal transmittal)
        {
            Transmittal = transmittal;
        }
        public Transmittal Transmittal { get; set; }
        public string RootUrl { get; set; }
    }
}
