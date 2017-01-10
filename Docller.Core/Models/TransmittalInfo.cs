using System;
using System.Collections.Generic;
namespace Docller.Core.Models
{
    public class TransmittalInfo : Transmittal
    {
        private readonly Dictionary<string, FileVersion> _files;
        private readonly Dictionary<long, long> _distributionInfo;
        public TransmittalInfo()
        {
            _files = new Dictionary<string, FileVersion>(StringComparer.InvariantCultureIgnoreCase);
            _distributionInfo = new Dictionary<long, long>();
        }
        public Dictionary<string, FileVersion> IssuedFilesInfo
        {
            get { return _files; }
        }

        public Dictionary<long, long> DistributionInfo
        {
            get { return _distributionInfo; }
        }
    }
}