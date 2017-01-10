using System;

namespace Docller.Core.Services
{
    public class DownloadCompletedEventArgs:EventArgs 
    {
        private readonly IDownloadProvider _downloadProvider;
        private readonly Exception _exception;

        public DownloadCompletedEventArgs(IDownloadProvider downloadProvider, Exception exception)
        {
            _downloadProvider = downloadProvider;
            _exception = exception;
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        public IDownloadProvider DownloadProvider
        {
            get { return _downloadProvider; }
        }
    }
}