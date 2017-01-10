using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Repository;
using Docller.Core.Storage;
using File = System.IO.File;

namespace Docller.Core.Images
{
    public class PreviewImageProvider : IPreviewImageProvider
    {
        private readonly IDirectDownloadProvider _downloadProvider;
        private readonly ILocalStorage _localStorage;
        
        public PreviewImageProvider(IDirectDownloadProvider downloadProvider)
        {
            _downloadProvider = downloadProvider;
            _localStorage = Factory.GetInstance<ILocalStorage>();
        }

        public void GeneratePreviews(long customerId, BlobBase blobBase)
        {
            IImageConverter converter = ImageConverters.Get(blobBase.FileExtension);
            if(converter is NullImageConverter)
                return;
            
            string internalName = blobBase.FileInternalName.ToString();
            string localFolder = this.GetLocalCacheFolder(customerId, blobBase);

            string lThumbFullName = GetLocalPreviewFileName(internalName, localFolder, PreviewType.LThumb);

            string originalFile = _downloadProvider.GetFileFromStorage(customerId, blobBase);
            ConvertToImage(converter, originalFile, lThumbFullName);
            UploadToStorage(customerId, blobBase, lThumbFullName);
            
            string pThumbFullName = GetLocalPreviewFileName(internalName, localFolder, PreviewType.PThumb);
            ResizeImage(lThumbFullName, pThumbFullName, ImageSize.Preview, ImageSize.Preview);
            UploadToStorage(customerId, blobBase, pThumbFullName);

            string sThumbFullName = GetLocalPreviewFileName(internalName, localFolder, PreviewType.SThumb);
            ResizeImage(lThumbFullName, sThumbFullName, ImageSize.Small, ImageSize.Small);
            UploadToStorage(customerId, blobBase, sThumbFullName);

            //Finally set the flag in the DB
            IStorageRepository storageRepository = Factory.GetRepository<IStorageRepository>(customerId);

            storageRepository.SetPreviewsTimestamp(blobBase.FileId, DateTime.UtcNow);

        }

        public void DeletePreviews(long customerId, BlobBase blobBase)
        {
            string localFolder = this.GetLocalCacheFolder(customerId, blobBase);
            //Delete local cache folder if it exits
            DirectoryInfo directoryInfo = new DirectoryInfo(localFolder);
            if (directoryInfo.Exists)
            {
                directoryInfo.Delete(true);
            }

            //Delete from Blob storage
            IBlobStorageProvider blobStorageProvider = Factory.GetInstance<IBlobStorageProvider>();
            string path = GetStoragePath(customerId, blobBase, blobStorageProvider.GetPathSeparator());
            blobStorageProvider.DeleteDirectory(path, Constants.PreviewImagesContainer);

            IStorageRepository storageRepository = Factory.GetRepository<IStorageRepository>(customerId);
            storageRepository.SetPreviewsTimestamp(blobBase.FileId, DateTime.MinValue);
        }

        public string GetPreview(long customerId, BlobBase blobBase, PreviewType previewType)
        {
            string path = GetLocalCacheFolder(customerId, blobBase);
            return GetPreview(customerId, blobBase, previewType, path);
        }

        public string GetZoomablePreview(long customerId, BlobBase blobBase)
        {
            string path = GetLocalCacheFolder(customerId, blobBase);
            string fullSizePath = GetPreview(customerId, blobBase, PreviewType.LThumb, path);
            IZoomableImageProvider zoomableImageProvider = Factory.GetInstance<IZoomableImageProvider>();
            return zoomableImageProvider.GenerateZoomableImage(fullSizePath,path);

        }

        public string GetTile(long customerId, BlobBase blobBase, string tile)
        {
            string path = GetLocalCacheFolder(customerId, blobBase);
            return string.Format("{0}\\zoomed_files\\{1}", path, tile);
        }

        private string GetPreview(long customerId, BlobBase blobBase, PreviewType previewType, string folderPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            if (directoryInfo.Exists)
            {
                FileInfo previewInfo = directoryInfo.GetFiles(string.Format("{0}_{1}.png", blobBase.FileInternalName, previewType)).FirstOrDefault();
                if (previewInfo != null && previewInfo.Exists)
                {
                    return previewInfo.FullName;
                }
            }
            return DownloadAndCache(customerId, blobBase, previewType, directoryInfo);
        } 

        private string DownloadAndCache(long customerId, BlobBase blobBase, PreviewType previewType, DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                directory.Create();
            }
            IBlobStorageProvider blobStorageProvider = Factory.GetInstance<IBlobStorageProvider>();
            string seprator = blobStorageProvider.GetPathSeparator();
            string fileName = string.Format("{0}_{1}.png", blobBase.FileInternalName, previewType);
             
            string pathToDownload = string.Format("{0}{1}{2}", GetStoragePath(customerId, blobBase,seprator),seprator,fileName);
            string targetFile = string.Format("{0}\\{1}", directory.FullName, fileName);
            using (
                FileStream target = new FileStream(targetFile,
                                                   FileMode.Create, FileAccess.Write))
            {
                blobStorageProvider.DownloadToStream(Constants.PreviewImagesContainer, pathToDownload, target);
            }
            return targetFile;
        }

        private string GetLocalPreviewFileName(string internalFileName, string localFolder, PreviewType previewType)
        {
            string thumbFile = GetPreviewFileName(internalFileName, previewType);
            string thumbFileFullPath = string.Format("{0}\\{1}", localFolder, thumbFile);
            return thumbFileFullPath;
        }
        private string GetPreviewFileName(string internalFileName, PreviewType previewType)
        {
            return string.Format("{0}_{1}.png", internalFileName, previewType);
        }

        private void ConvertToImage(IImageConverter converter, string file, string imageFile)
        {
            converter.Convert(file, imageFile);
        }

        private  void ResizeImage(string inputFile, string outputfile, double height, double width)
        {
            ImageResizer.ResizeImage(inputFile,outputfile,height,width);
        }


        private string GetLocalCacheFolder(long customerId, BlobBase blobBase)
        {
            string folderPath = string.Format("{0}\\{1}", Constants.PreviewImagesContainer,
                                              GetStoragePath(customerId, blobBase, "\\"));

            return _localStorage.EnsureCacheFolder(folderPath);
        }

        private string GetStoragePath(long customerId,BlobBase blobBase, string seprator)
        {
            return string.Format("c{0}{3}p{1}{3}f{2}", customerId,
                                 blobBase.Project.ProjectId, blobBase.FileId, seprator);
        }

        private void UploadToStorage(long customerId, BlobBase blobBase, string fileToUpload)
        {
            FileInfo fileInfo = new FileInfo(fileToUpload);
            using (FileStream stream = fileInfo.OpenRead())
            {
                IBlobStorageProvider blobStorageProvider = Factory.GetInstance<IBlobStorageProvider>();
                string path = GetStoragePath(customerId, blobBase, blobStorageProvider.GetPathSeparator());
                blobStorageProvider.UploadFile(Constants.PreviewImagesContainer, path, fileInfo.Name, stream,
                                               MIMETypes.Current[".png"]);
            }
        }

        

        
    }
}