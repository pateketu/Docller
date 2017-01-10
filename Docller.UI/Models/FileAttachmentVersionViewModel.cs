using Docller.Core.Models;

namespace Docller.UI.Models
{
    public class FileAttachmentVersionViewModel : FileAttachmentViewModel
    {
        public FileAttachmentVersionViewModel(FileAttachment version)
            : base(version)
        {
            this.VersionPath = ((FileAttachmentVersion)version).VersionPath;
        }

        public string VersionPath { get; set; }
    }
}