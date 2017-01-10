using System.IO;
using System.IO.Compression;
using System.Linq;
using Docller.Core.Common;
using Docller.Core.Models;

namespace Docller.Core.Services
{
    public class FileVersionDownloader : MultipleFileDownloadProvider
    {
        private readonly FileVersion _fileversion;
        public FileVersionDownloader(FileVersion fileVersion,string fileName, long customerId):base(fileName,customerId)
        {
            _fileversion = fileVersion;        
        }

        protected override void DownloadFiles(ZipArchive archive, IClientConnection clientConnection)
        {
            ZipArchiveEntry entry = archive.CreateEntry(_fileversion.FileName, CompressionLevel.Optimal);
            using (Stream stream = entry.Open())
            {
                StorageService.DownloadToStream(stream, _fileversion, clientConnection);
            }
            if (_fileversion.Attachments != null && _fileversion.Attachments.Count == 1)
            {
                DownloadAttachment(_fileversion.Attachments.First(), archive, clientConnection);
            }
        }

    }
}