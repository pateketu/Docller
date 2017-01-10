using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docller.Core.Models
{
    public class TransmittalUserComparer : IEqualityComparer<TransmittalUser>
    {
        public bool Equals(TransmittalUser x, TransmittalUser y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return x.UserId == y.UserId;
        }

        public int GetHashCode(TransmittalUser obj)
        {
            return obj.UserId.GetHashCode();
        }
    }
}
