using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Docller.Core.Models;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Mappers.StoredProcMappers
{
    public class FilePreUploadInfoMapper:IResultSetMapper<File> 
    {
        private readonly IRowMapper<File> _fileMapper;
        private readonly IRowMapper<FileAttachment> _attachmentMapper;
        private readonly IRowMapper<Folder> _folderMapper;
        private readonly IRowMapper<Project> _projectMapper;

        public FilePreUploadInfoMapper()
        {
            this._fileMapper = MapBuilder<File>.MapNoProperties()
                                                .MapByName(x=>x.FileId)
                                                .MapByName(x => x.FileInternalName)
                                                .MapByName(x => x.FileName)
                                                .MapByName(x => x.IsExistingFile)
                                                .MapByName(x=>x.BaseFileName)
                                                .MapByName(x=>x.DocNumber)
                                                .Build();
            this._attachmentMapper = MapBuilder<FileAttachment>.MapNoProperties()
                                                               .MapByName(x=>x.FileId)
                                                               .MapByName(x => x.FileName)
                                                               .MapByName(x=>x.BaseFileName)
                                                               .MapByName(x => x.ParentFile)
                                                               .MapByName(x => x.IsExistingFile).Build();
            this._folderMapper = MapBuilder<Folder>.MapNoProperties()
                                                    .MapByName(x => x.FolderId)
                                                    .MapByName(x => x.FolderInternalName)
                                                    .MapByName(x => x.FullPath).Build();

            this._projectMapper = MapBuilder<Project>.MapNoProperties()
                                                    .MapByName(x => x.ProjectId)
                                                    .MapByName(x => x.BlobContainer).Build();

        } 

        public IEnumerable<File> MapSet(IDataReader reader)
        {
            Dictionary<Guid,File> files = new Dictionary<Guid, File>();

            using (reader)
            {
                Project project = this.GetProject(reader);

                Folder folder = null;
                if(reader.NextResult())
                {
                    folder = this.GetFolder(reader);    
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        File file = _fileMapper.MapRow(reader);
                        file.Project = project;
                        file.Folder = folder;
                        file.Attachments = new List<FileAttachment>();
                        files.Add(file.FileInternalName, file);
                    }
                }

                if(reader.NextResult())
                {
                    while (reader.Read())
                    {
                        FileAttachment attachment = _attachmentMapper.MapRow(reader);
                        attachment.Project = project;
                        attachment.Folder = folder;
                        if(files.ContainsKey(attachment.ParentFile))
                        {
                            files[attachment.ParentFile].Attachments.Add(attachment);
                        }
                    }
                }
            }

            return files.Values.ToList();
        }

        private Folder GetFolder(IDataReader reader)
        {
            if(reader.Read())
            {
                return this._folderMapper.MapRow(reader);
            }

            return null;
        }

        private Project GetProject(IDataReader reader)
        {
            if(reader.Read())
            {
                return this._projectMapper.MapRow(reader);
            }
            return null;
        }
    }

    

}
