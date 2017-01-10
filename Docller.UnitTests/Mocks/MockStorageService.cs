using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Models;
using Docller.Core.Repository;
using Docller.Core.Services;
using Docller.Core.Storage;

namespace Docller.Tests.Mocks
{
    public class MockStorageService:StorageService  
    {
        public MockStorageService(IStorageRepository repository, IBlobStorageProvider blobStorageProvider) : base(repository, blobStorageProvider)
        {
        }

        public override IEnumerable<File> GetPreUploadInfo(long projectId, long folderId, string[] fileNames, bool attachCADFilesToPdfs, bool patternMatchForVersions)
        {
            List<File> files = GetFilesWithAttachments(fileNames, attachCADFilesToPdfs,null);
            return files;
        }

       
    }

   
}
