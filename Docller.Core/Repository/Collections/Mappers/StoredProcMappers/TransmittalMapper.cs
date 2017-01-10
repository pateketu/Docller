using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Docller.Core.Models;
using Docller.Core.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Mappers.StoredProcMappers
{
    public class TransmittalMapper:IResultSetMapper<Transmittal>
    {
        private readonly IRowMapper<Transmittal> _transmittaMapper;
        private readonly IRowMapper<TransmittalUser> _userMapper;
        private readonly IRowMapper<TransmittedFile> _fileMapper;
        private readonly IRowMapper<TransmittedFileVersion> _fileVersionMapper;

        public TransmittalMapper()
        {
            _transmittaMapper =
                MapBuilder<Transmittal>.MapNoProperties()
                                       .MapByName(x => x.TransmittalNumber)
                                       .MapByName(x => x.TransmittalId)
                                       .MapByName(x => x.Subject)
                                       .MapByName(x => x.Message)
                                       .MapByName(x => x.IsDraft)
                                       .MapByName(x => x.CreatedDate)
                                       .Map(x => x.TransmittalStatus)
                                       .WithFunc(
                                           r =>
                                           new Status()
                                               {
                                                   StatusId = r.GetNullableLong(5),
                                                   StatusText = r.GetNullableString(6)
                                               })
                                       .Map(x => x.CreatedBy)
                                       .WithFunc(
                                           record =>
                                           new User() {FirstName = record.GetString(8), LastName = record.GetString(9)})
                                        .MapByName(x => x.BlobContainer)
                                       .Build();

            _userMapper =
                MapBuilder<TransmittalUser>.MapNoProperties()
                                           .MapByName(x=>x.UserId)
                                           .MapByName(x => x.FirstName)
                                           .MapByName(x => x.LastName)
                                           .MapByName(x=>x.Email)
                                           .Map(x => x.IsCced)
                                           .ToColumn("Cced")
                                           .Map(x=>x.Company).WithFunc(
                                               record => new Company() {CompanyName = record.GetString(5)})
                                           .Build();

            _fileMapper =
                MapBuilder<TransmittedFile>.MapNoProperties()
                                           .MapByName(x => x.FileId)
                                           .MapByName(x => x.FileInternalName)
                                           .MapByName(x => x.FileName)
                                           .MapByName(x => x.Title)
                                           .MapByName(x => x.Revision)
                                           .Map(x => x.RevisionNumber).ToColumn("CurrentRevision")
                                           .Map(x => x.Status)
                                           .ToColumn("StatusText")
                                           .Map(x => x.Folder).WithFunc(r => new Folder() {FullPath = r.GetString(7)})
                                           .Build();

            _fileVersionMapper =
                MapBuilder<TransmittedFileVersion>.MapNoProperties()
                                                  .MapByName(x => x.FileId)
                                                  .MapByName(x => x.VersionPath)
                                                  .MapByName(x => x.FileInternalName)
                                                  .MapByName(x => x.FileName)
                                                  .MapByName(x => x.Title)
                                                  .MapByName(x => x.Revision)
                                                  .MapByName(x => x.RevisionNumber)
                                                  .Map(x => x.Status)
                                                  .ToColumn("StatusText")
                                                  .Map(x => x.Folder)
                                                  .WithFunc(r => new Folder() {FullPath = r.GetString(8)})
                                                  .Build();
        }

        public IEnumerable<Transmittal> MapSet(IDataReader reader)
        {

            Transmittal transmittal;
            using (reader)
            {
                transmittal = MapSetTransmittal(reader);
                if (transmittal != null)
                {
                    
                    transmittal.Files = new List<TransmittedFile>();
                    
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            TransmittedFile f = _fileMapper.MapRow(reader);
                            f.Project = new Project() {BlobContainer = transmittal.BlobContainer};
                            transmittal.Files.Add(f);
                        }
                    }

                    //We need to check for next result, as we might not have the last result returning back from stored proc
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            TransmittedFileVersion version = _fileVersionMapper.MapRow(reader);
                            version.Project = new Project() {BlobContainer = transmittal.BlobContainer};
                            transmittal.Files.Add(version);
                        }
                    }
                }
            }
            yield return transmittal;
        }

        internal Transmittal MapSetTransmittal(IDataReader reader)
        {
            if (reader.Read())
            {
                Transmittal transmittal = _transmittaMapper.MapRow(reader);
                transmittal.Distribution = new List<TransmittalUser>();

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        transmittal.Distribution.Add(_userMapper.MapRow(reader));
                    }
                }
                return transmittal;
            }
            return null;
        }
        
    }
}
