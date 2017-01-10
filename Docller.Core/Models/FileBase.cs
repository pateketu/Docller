namespace Docller.Core.Models
{
    public abstract class FileBase : BlobBase
    {
        public string Title { get; set; }
        public string DocNumber { get; set; }
        public string Revision { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public long StatusId { get; set; }
    }
}