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
    public class AttachmentMapper : IResultSetMapper<FileAttachment> 
    {
        private readonly IRowMapper<FileAttachment> _fileAttachmentMapper;
        private readonly IRowMapper<FileAttachmentVersion> _fileAttachmentVersions;
        
        public AttachmentMapper()
        {
            _fileAttachmentMapper = DefaultMappers.ForFileAttachment;
            _fileAttachmentVersions = MapBuilder<FileAttachmentVersion>.MapNoProperties()
                                                                       .MapByName(x => x.FileId)
                                                                       .MapByName(x => x.RevisionNumber)
                                                                       .MapByName(x => x.VersionPath)
                                                                       .MapByName(x => x.FileName)
                                                                       .MapByName(x => x.FileExtension)
                                                                       .MapByName(x => x.ContentType)
                                                                       .MapByName(x=>x.CreatedDate)
                                                                       .Map(x => x.CreatedBy)
                                                                       .WithFunc(
                                                                           reader =>
                                                                           new User()
                                                                               {
                                                                                   FirstName = reader.GetNullableString(7),
                                                                                   LastName = reader.GetNullableString(8)
                                                                               })
                                                                       .Build();
        }

        public IEnumerable<FileAttachment> MapSet(IDataReader reader)
        {
            FileAttachment attachment = null;
    
            using (reader)
            {
                if(reader.Read())
                {
                    attachment = _fileAttachmentMapper.MapRow(reader);
                    
                }

                if (reader.NextResult() && attachment != null)
                {
                    attachment.Versions = new List<FileAttachmentVersion>();
                    while (reader.Read())
                    {
                        FileAttachmentVersion v = this._fileAttachmentVersions.MapRow(reader);
                        v.ParentFile = attachment.ParentFile;
                        v.Folder = attachment.Folder;
                        v.Project = attachment.Project;
                        if (!string.IsNullOrEmpty(v.VersionPath))
                        {
                            attachment.Versions.Add(v);
                        }
                    }                    
                }
                
            }
            return new List<FileAttachment>() { attachment };
        }
    }
}
