using System;

namespace Docller.Core.Services
{
    public class DownloadState
    {
        public IDownloadProvider DownloadProvider { get; set; }
        public Exception Exception { get; set; }
    }
}