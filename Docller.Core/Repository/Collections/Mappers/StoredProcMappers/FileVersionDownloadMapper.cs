using System.Collections.Generic;
using System.Data;
using Docller.Core.Models;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Mappers.StoredProcMappers
{
    public class FileVersionDownloadMapper : IResultSetMapper<FileVersion>
    {
        private readonly IRowMapper<FileVersion> _fileVersionMapper;
        private readonly IRowMapper<FileAttachmentVersion> _attachmentMapper;
        public FileVersionDownloadMapper()
        {
            _fileVersionMapper = DefaultMappers.ForFileVersionDownload;
            this._attachmentMapper = MapBuilder<FileAttachmentVersion>.MapNoProperties()
                                                              .MapByName(x => x.FileId)
                                                              .MapByName(x => x.FileName)
                                                              .MapByName(x => x.FileExtension)
                                                              .MapByName(x => x.ContentType)
                                                              .MapByName(x => x.FileSize)
                                                              .MapByName(x => x.RevisionNumber)
                                                              .MapByName(x=>x.VersionPath)
                                                              .Build(); 
        }
        public IEnumerable<FileVersion> MapSet(IDataReader reader)
        {
            FileVersion version = null;
            while (reader.Read())
            {
                version = this._fileVersionMapper.MapRow(reader);
                version.Attachments = new List<FileAttachmentVersion>();
            }
            if (reader.NextResult() && version != null)
            {
                while (reader.Read())
                {
                    FileAttachmentVersion attachment = this._attachmentMapper.MapRow(reader);

                    attachment.Folder = version.Folder;
                    attachment.Project = version.Project;
                    attachment.ParentFile = version.FileInternalName;
                    version.Attachments.Add(attachment);
                    
                }
            }
            return new List<FileVersion>(){version};
        }
    }
}
