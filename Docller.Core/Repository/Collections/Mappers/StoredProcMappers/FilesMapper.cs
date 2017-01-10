using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docller.Core.Common;
using Docller.Core.Models;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Mappers.StoredProcMappers
{
    public class FilesMapper:IResultSetMapper<File>
    {
        private readonly IRowMapper<File> _fileMapper;
        private IRowMapper<FileAttachment> _attachmentMapper;

        public FilesMapper(IRowMapper<File> fileMapper)
        {
            this._fileMapper = fileMapper;
            this.InitAttachmentMapper();
        }

        public FilesMapper()
        {
            this._fileMapper = MapBuilder<File>.MapNoProperties()
                                               .MapByName(x => x.FileId)
                                               .MapByName(x => x.FileInternalName)
                                               .MapByName(x => x.FileName)
                                               .MapByName(x => x.FileExtension)
                                               .MapByName(x => x.ContentType)
                                               .MapByName(x => x.VersionCount)
                                               .MapByName(x => x.FileSize)
                                               .MapByName(x => x.DocNumber)
                                               .MapByName(x => x.Revision)
                                               .MapByName(x => x.Title)
                                               .MapByName(x => x.Notes)
                                               .MapByName(x => x.ThumbnailUrl)
                                               .MapByName(x => x.PreviewUrl)
                                               .MapByName(x => x.CreatedDate)
                                               .MapByName(x => x.Status)
                                               .Map(x => x.Folder)
                                               .WithFunc(x => new Folder() {FullPath = x.GetString(15)})
                                               .Map(x => x.CreatedBy)
                                               .WithFunc(
                                                   x =>
                                                   new User()
                                                       {
                                                           UserName = x.GetString(16),
                                                           FirstName = x.GetNullableString(17),
                                                           LastName = x.GetNullableString(18)
                                                       })
                                               .Map(x => x.HasVersions).WithFunc(x => x.GetInt32(19) > 1)
                                               .Map(x => x.HasTransmittals).WithFunc(x => x.GetInt32(20) > 0)
                                               .Map(x => x.PreviewsTimeStamp).WithFunc(delegate(IDataRecord record)
                                                   {
                                                       DateTime timeStamp = record.GetNullableDateTime(21);
                                                       if (timeStamp != DateTime.MinValue)
                                                       {
                                                           return timeStamp.Ticks;
                                                       }
                                                       return 0;
                                                   })
                                               .Build();

            this.InitAttachmentMapper();
        }

        private void InitAttachmentMapper()
        {
            this._attachmentMapper = MapBuilder<FileAttachment>.MapNoProperties()
                                                              .MapByName(x => x.FileId)
                                                              .MapByName(x => x.FileName)
                                                              .MapByName(x => x.FileExtension)
                                                              .MapByName(x => x.ContentType)
                                                              .MapByName(x => x.FileSize)
                                                              .MapByName(x => x.RevisionNumber)
                                                              
                                                              .Build();   
        }
        public virtual IEnumerable<File> MapSet(IDataReader reader)
        {
            Dictionary<long, File> dictionary = new Dictionary<long, File>();
            Files files = new Files();
            
            using (reader)
            {
                reader.Read();
                files.TotalCount = reader.GetInt32(0);
                if (reader.NextResult())
                {
                    dictionary = this.Map(reader);
                }
            }
            files.AddRange(dictionary.Values);
            return files;
        }

        protected Dictionary<long, File> Map(IDataReader reader)
        {
            Dictionary<long, File> dictionary = new Dictionary<long, File>();
            while (reader.Read())
            {
                File file = this._fileMapper.MapRow(reader);
                file.Attachments = new List<FileAttachment>();
                dictionary.Add(file.FileId, file);
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    FileAttachment attachment = this._attachmentMapper.MapRow(reader);
                    File f = dictionary[attachment.FileId];
                    attachment.Folder = f.Folder;
                    attachment.Project = f.Project;
                    attachment.ParentFile = f.FileInternalName;
                    f.Attachments.Add(attachment);
                    //dictionary[attachment.FileId].Attachments.Add(attachment);
                }
            }
            return dictionary;
        }
    }
}
