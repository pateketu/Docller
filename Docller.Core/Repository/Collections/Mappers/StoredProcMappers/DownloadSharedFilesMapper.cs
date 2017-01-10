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
    public class DownloadSharedFilesMapper : IResultSetMapper<Transmittal>
    {
        private readonly IRowMapper<TransmittedFile> _fileMapper;
        private readonly IRowMapper<TransmittedFileVersion> _fileVersionMapper;
        private readonly IRowMapper<Transmittal> _transmittaMapper;
        public DownloadSharedFilesMapper()
        {

            _transmittaMapper =
                MapBuilder<Transmittal>.MapNoProperties()
                                       .MapByName(x => x.TransmittalId)
                                       .MapByName(x => x.Subject)
                                       .MapByName(x => x.BlobContainer)
                                       .Build();
        
            _fileMapper =
                MapBuilder<TransmittedFile>.MapNoProperties()
                                           .MapByName(x => x.FileId)
                                           .MapByName(x => x.FileInternalName)
                                           .MapByName(x => x.FileName)
                                           .Map(x => x.RevisionNumber).ToColumn("CurrentRevision")
                                           .MapByName(x => x.FileSize)
                                           .Map(x => x.Folder).WithFunc(r => new Folder() {FullPath = r.GetString(5)})
                                           .Build();

            _fileVersionMapper =
                MapBuilder<TransmittedFileVersion>.MapNoProperties()
                                                  .MapByName(x => x.FileId)
                                                  .MapByName(x => x.FileInternalName)
                                                  .MapByName(x => x.VersionPath)
                                                  .MapByName(x => x.FileName)
                                                  .MapByName(x => x.RevisionNumber)
                                                  .MapByName(x => x.FileSize)
                                                  .Map(x => x.Folder)
                                                  .WithFunc(r => new Folder() {FullPath = r.GetString(5)})
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
                            f.Project = new Project() { BlobContainer = transmittal.BlobContainer };
                            transmittal.Files.Add(f);
                        }
                    }

                    //We need to check for next result, as we might not have the last result returning back from stored proc
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            TransmittedFileVersion version = _fileVersionMapper.MapRow(reader);
                            version.Project = new Project() { BlobContainer = transmittal.BlobContainer };
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
                return transmittal;
            }
            return null;
        }
    }
}
