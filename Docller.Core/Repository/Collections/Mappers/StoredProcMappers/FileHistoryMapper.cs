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
    public class FileHistoryMapper : IResultSetMapper<FileHistory> 
    {
        private readonly IRowMapper<File> _fileMapper;
        private readonly IRowMapper<FileVersion> _fileVersion;
        private readonly IRowMapper<Transmittal> _transmittaMapper;

        public FileHistoryMapper()
        {
            this._fileMapper = MapBuilder<File>.MapNoProperties()
                                                .MapByName(x=>x.FileId)
                                               .MapByName(x => x.FileName)
                                               .MapByName(x=>x.FileExtension)
                                               .Build();
            this._fileVersion = MapBuilder<FileVersion>.MapNoProperties()
                                                       .MapByName(x => x.FileName)
                                                       .MapByName(x => x.Title)

                                                       .MapByName(x => x.CreatedDate)
                                                       .MapByName(x => x.RevisionNumber)
                                                       .MapByName(x => x.Revision)
                                                       .MapByName(x => x.VersionPath)
                                                       .Map(x => x.Status).ToColumn("StatusText")
                                                       .Map(x => x.CreatedBy)
                                                       .WithFunc(
                                                           record =>
                                                           new User()
                                                               {
                                                                   FirstName = record.GetString(7),
                                                                   LastName = record.GetString(8)
                                                               })
                                                       .MapByName(x => x.TransmittalId)
                                                       .Map(x => x.Attachments).WithFunc(delegate(IDataRecord record) 
                                                                                            {
                                                                                                if (!record.IsDBNull(10))
                                                                                                {
                                                                                                    return new List<FileAttachmentVersion>()
                                                                                                        { 
                                                                                                            new FileAttachmentVersion() 
                                                                                                            { 
                                                                                                                FileName = record.GetString(10), 
                                                                                                                FileExtension = record.GetString(11)
                                                                                                            } 
                                                                                                        };
                                                                                                }
                                                                                                return null; 
                                                                                            })
                                                       .Build();

            _transmittaMapper =
                MapBuilder<Transmittal>.MapNoProperties()
                                       .MapByName(x => x.TransmittalId)
                                       .MapByName(x => x.TransmittalNumber)
                                       .MapByName(x => x.Subject)
                                       .MapByName(x => x.CreatedDate)
                                       .Map(x => x.TransmittalStatus)
                                       .WithFunc(
                                           r =>
                                           new Status()
                                               {
                                                   StatusText = r.GetNullableString(4)
                                               })
                                       .Map(x => x.CreatedBy)
                                       .WithFunc(
                                           record =>
                                           new User() {FirstName = record.GetString(5), LastName = record.GetString(6)})
                                       .Build();

        }

        public IEnumerable<FileHistory> MapSet(IDataReader reader)
        {
            FileHistory history = new FileHistory() ;
    
            using (reader)
            {
                if(reader.Read())
                {
                    history.File = _fileMapper.MapRow(reader);
                    history.File.Versions = new List<FileVersion>();
                }

                if (reader.NextResult())
                {
                    List<Transmittal> transmittals = new List<Transmittal>();
                    while (reader.Read())
                    {
                        transmittals.Add(this._transmittaMapper.MapRow(reader));
                    }
                    history.Transmittals = transmittals;
                }
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        history.File.Versions.Add(_fileVersion.MapRow(reader));
                    }
                }
            }
            return new List<FileHistory>() {history};
        }
    }
}
