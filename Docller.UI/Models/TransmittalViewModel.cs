using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Docller.Common;
using Docller.Core.Common;
using Docller.Core.Models;
using Newtonsoft.Json;

namespace Docller.Models
{
    public class TransmittalViewModel
    {
        private string _fileJson;
        private long _statusId;
        public TransmittalViewModel(IEnumerable<File> files, IEnumerable<SelectListItem> statuses )
        {
            this.To = new SubscriberItemCollection();
            this.Cc = new SubscriberItemCollection();
            this.TransmittedFiles = new List<TransmittedFile>();
            this.StatusList = statuses;
            this.Init(files);
        }

        public TransmittalViewModel(Transmittal transmittal, IEnumerable<SelectListItem> statuses )
        {
            this.To = new SubscriberItemCollection();
            this.Cc = new SubscriberItemCollection();
            this.StatusList = statuses;
            this.Init(transmittal);
        }
        public TransmittalViewModel()
        {

            this.To =  new SubscriberItemCollection();
            this.Cc = new SubscriberItemCollection();
            this.StatusList = new List<SelectListItem>();
        }

        public TransmittalViewModel(IEnumerable<SelectListItem> statuses)
        {

            this.To = new SubscriberItemCollection();
            this.Cc = new SubscriberItemCollection();
            this.StatusList = statuses;
        }
        public long TransmittalId { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "To")]
        [RequiredForTransmittal]
        public SubscriberItemCollection To { get; set; }

        public SubscriberItemCollection Cc { get; set; }
        
        [DataType(DataType.Text)]
        [RequiredForTransmittal]
        [Display(Name = "Transmittal Number")]
        public string TransmittalNumber { get; set; }
        
        [DataType(DataType.Text)]
        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }
        [Display(Name = "Message")]
        public string Message { get; set; }
        
        public List<TransmittedFile> TransmittedFiles { get; set; }

        [Range(1,Int32.MaxValue,ErrorMessage = "You must add files to your transmittal")]
        public int FileCount
        {
            get { return this.TransmittedFiles != null ? this.TransmittedFiles.Count : 0; }
        }

        public string Action { get; set; }

        [RequiredForTransmittal]
        [Display(Name = "Status")]
        public long StatusId
        {
            get { return _statusId; }
            set 
            {
                _statusId = value;
                var selected = (from s in this.StatusList
                                where s.Value == _statusId.ToString(CultureInfo.InvariantCulture)
                                select s).FirstOrDefault();
                if (selected != null)
                {
                    selected.Selected = true;
                }


            }
        }
        public string FileJson {
            get
            {
                if (string.IsNullOrEmpty(_fileJson))
                {
                    JsonNetResult result = new JsonNetResult {Data = this.TransmittedFiles};
                    _fileJson = result.GetRawJson();
                }
                return _fileJson;
            }
        }
        public IEnumerable<SelectListItem> StatusList { get; set; } 

        private void Init(IEnumerable<File> files)
        {
            foreach (File file in files)
            {
                this.TransmittedFiles.Add(new TransmittedFile()
                    {
                        FileId = file.FileId,
                        FileInternalName = file.FileInternalName,
                        FileName = file.FileName,
                        Status = file.Status,
                        Revision = file.Revision,
                        Title = file.Title
                    });
            }

            
        }

        private void Init(Transmittal transmittal)
        {

            this.Subject = transmittal.Subject;
            this.Message = transmittal.Message;
            this.TransmittalNumber = transmittal.TransmittalNumber;
            this.TransmittedFiles = transmittal.Files;
            this.TransmittalId = transmittal.TransmittalId;
            if (transmittal.Distribution != null)
            {
                var to = from d in transmittal.Distribution
                         where d.IsCced == false
                         select d;
                this.To = new SubscriberItemCollection(to);
                var cc = from d in transmittal.Distribution
                         where d.IsCced == true
                         select d;
                this.Cc = new SubscriberItemCollection(cc);
            }

        }
        public Transmittal Convert(IDocllerContext context)
        {
            Transmittal transmittal = new Transmittal
                {
                    TransmittalNumber = this.TransmittalNumber,
                    Subject = this.Subject,
                    Message = this.Message,
                    CreatedBy = new User() {UserName = context.UserName},
                    ProjectId = context.ProjectId,
                    TransmittalStatus = new Status() {StatusId = this.StatusId},
                    Files = this.TransmittedFiles,
                    TransmittalId = this.TransmittalId
                };
            return transmittal;
        }

      

       
    }

}