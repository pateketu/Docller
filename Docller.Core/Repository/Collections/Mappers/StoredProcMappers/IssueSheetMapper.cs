using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docller.Core.Models;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Mappers.StoredProcMappers
{
    public class IssueSheetMapper:IResultSetMapper<IssueSheet>
    {
        private readonly IRowMapper<TransmittedFile> _transmittedFilesMapper;
        private readonly IRowMapper<TransmittalInfo> _transmittalInfoMapper;
        private readonly IRowMapper<TransmittalUser> _transmittalUserMapper;
        private readonly IRowMapper<FileVersion> _fileVersionMapper;
        private readonly IRowMapper<Status> _statusMapper;
        private readonly IRowMapper<IssueSheet> _issueSheetMapper; 
        public IssueSheetMapper()
        {
            _transmittedFilesMapper =
                MapBuilder<TransmittedFile>.MapNoProperties()
                                           .MapByName(x => x.FileId)
                                           .MapByName(x => x.FileName)
                                           .MapByName(x => x.Title)
                                           .MapByName(x => x.Revision)
                                           .Build();
            
            _transmittalInfoMapper =
                MapBuilder<TransmittalInfo>.MapNoProperties()
                                           .MapByName(x => x.TransmittalId)
                                           .Map(x => x.CreatedDate)
                                           .ToColumn("TransmittalDate")
                                           .Map(x => x.TransmittalStatus)
                                           .WithFunc(
                                               record =>
                                               new Status()
                                                   {
                                                       StatusId = record.GetInt64(6),
                                                       StatusText = record.GetString(7)
                                                   })
                                           .Build();

            _transmittalUserMapper =
                MapBuilder<TransmittalUser>.MapNoProperties()
                                           .MapByName(x=>x.TransmittalId) 
                                           .MapByName(x=>x.UserId)
                                           .MapByName(x => x.FirstName)
                                           .MapByName(x => x.LastName)
                                           .MapByName(x=>x.Email)
                                           .Map(x => x.Company)
                                           .WithFunc(
                                               r =>
                                               new Company() {CompanyId = r.GetInt64(5), CompanyName = r.GetString(6)})
                                           .Build();

            _fileVersionMapper =
                MapBuilder<FileVersion>.MapNoProperties()
                                       .MapByName(x => x.FileName)
                                       .MapByName(x => x.Title)
                                       .MapByName(x => x.Revision)
                                       .MapByName(x => x.RevisionNumber)
                                       .Build();

            _statusMapper = MapBuilder<Status>.MapNoProperties()
                                              .MapByName(x => x.StatusId)
                                              .MapByName(x => x.StatusText)
                                              .MapByName(x => x.StatusLongText)
                                              .Build();

            _issueSheetMapper = MapBuilder<IssueSheet>.MapNoProperties()
                                                      .MapByName(x => x.TransmittalId)
                                                      .MapByName(x => x.TransmittalNumber)
                                                      .MapByName(x => x.ProjectName)
                                                      .MapByName(x=>x.BlobContainer)
                                                      .Build();
        }

        public IEnumerable<IssueSheet> MapSet(IDataReader reader)
        {
            IssueSheet issueSheet = null;
            using (reader)
            {
                issueSheet = MapSetSingle(reader);

            }

            yield return issueSheet;
        }

       internal IssueSheet MapSetSingle(IDataReader reader)
       {
           IssueSheet issueSheet = new IssueSheet();
           Dictionary<long, TransmittedFile> transmittedFiles = new Dictionary<long, TransmittedFile>();
           Dictionary<long, TransmittalInfo> transmittalInfos = new Dictionary<long, TransmittalInfo>();
           if (reader.Read())
           {
               issueSheet = _issueSheetMapper.MapRow(reader);
           }

           if (reader.NextResult())
           {
               while (reader.Read())
               {
                   TransmittedFile tf = this._transmittedFilesMapper.MapRow(reader);
                   transmittedFiles.Add(tf.FileId, tf);
               }
           }

           if (reader.NextResult())
           {
               while (reader.Read())
               {
                   long transmittalId = reader.GetInt64(0);
                   TransmittalInfo tInfo;
                   if (transmittalInfos.ContainsKey(transmittalId))
                   {
                       tInfo = transmittalInfos[transmittalId];
                   }
                   else
                   {
                       tInfo = this._transmittalInfoMapper.MapRow(reader);
                       transmittalInfos.Add(tInfo.TransmittalId, tInfo);
                   }
                   FileVersion fVersion = this._fileVersionMapper.MapRow(reader);
                   if (!tInfo.IssuedFilesInfo.ContainsKey(fVersion.FileName))
                   {
                       tInfo.IssuedFilesInfo.Add(fVersion.FileName, fVersion);
                   }

               }
           }
           if (reader.NextResult())
           {
               issueSheet.Distribution = new List<TransmittalUser>();
               while (reader.Read())
               {
                   TransmittalUser user = this._transmittalUserMapper.MapRow(reader);
                   issueSheet.Distribution.Add(user);
                   transmittalInfos[user.TransmittalId].DistributionInfo.Add(user.UserId, user.UserId);
               }
           }

           if (reader.NextResult())
           {
               issueSheet.AllStatus = new List<Status>();
               while (reader.Read())
               {
                   issueSheet.AllStatus.Add(this._statusMapper.MapRow(reader));
               }
           }
           issueSheet.IssuedFiles = transmittedFiles.Values.ToList();
           issueSheet.Transmittals = transmittalInfos.Values.ToList();
           return issueSheet;
       }
    }
}
