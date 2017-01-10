using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Docller.Core.Common;
using Docller.Core.Models;

namespace Docller.UI.Models
{
    public class FileAttachmentViewModel
    {
        public FileAttachmentViewModel()
        {
            
        }

        public FileAttachmentViewModel(long fileId)
        {
            this.FileId = fileId;
        }
        public FileAttachmentViewModel(FileAttachment fileAttachment)
        {

            if (fileAttachment != null)
            {
                this.FileId = fileAttachment.FileId;
                this.FileName = fileAttachment.FileName;
                this.Extension = fileAttachment.FileExtension;
                this.CreatedBy = fileAttachment.CreatedBy.DisplayName;
                this.FormattedCreatedDate = fileAttachment.CreatedDate.ToShortDateString().Replace("/", ".");
                this.Icon = string.Format("/Images/filetype-icons/32X32/{0}",
                                          FileTypeIconsFactory.Current.Medium[fileAttachment.FileExtension]);
                this.RevisionNumber = fileAttachment.RevisionNumber;
                Initversions(fileAttachment.Versions);
            }
            
        }
        public int TotalChunks { get; set; }
        public int CurrentChunk { get; set; }
        public Stream FileStream { get; set; }
        public long FileId { get; set; }
        public int RevisionNumber { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string CreatedBy { get; set; }
        public string Icon { get; set; }
        public string FormattedCreatedDate { get; set; }
        public List<FileAttachmentViewModel> Versions { get; set; }
        public decimal FileSize { get; set; }
        public long ProjectId { get; set; }
        public long FolderId { get; set; }

        private void Initversions(IEnumerable<FileAttachmentVersion> versions)
        {
            if (versions != null)
            {
                Versions = new List<FileAttachmentViewModel>();

                foreach (var version in versions)
                {
                    Versions.Add(new FileAttachmentViewModel(version));
                }

            }
        }

        public FileAttachment GetFileAttachment()
        {
            return new FileAttachment()
                {
                    FileSize = this.FileSize,
                    FileName = this.FileName,
                    FileId = this.FileId,
                    Project = new Project() {ProjectId = this.ProjectId},
                    Folder = new Folder() {FolderId = this.FolderId}
                };


        }


    }
}