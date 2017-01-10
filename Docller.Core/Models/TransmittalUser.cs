using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docller.Core.Models
{
    public class TransmittalUser:User
    {
        public long TransmittalId { get; set; }
        public bool IsCced { get; set; }
    }
}
