using System.Collections.Generic;
using System.IO;
using Docller.Core.Common;
using File = Docller.Core.Models.File;

namespace Docller.Core.Services
{
    public interface IDownloadProvider
    {
        DownloadStatus DownloadToStream(Stream target, IClientConnection clientConnection);
        string ContentType { get; }
        string FileName { get; }
        void PrepareDownload(IClientConnection clientConnection);
        void CleanUp();

    }
}