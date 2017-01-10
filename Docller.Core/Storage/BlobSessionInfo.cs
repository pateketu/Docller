using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Docller.Core.Storage
{
    public class BlobSessionInfo
    {
        public CloudBlockBlob BlockBlob { get; set; }
        public string SnapshotVersion { get; set; }
    }
}
