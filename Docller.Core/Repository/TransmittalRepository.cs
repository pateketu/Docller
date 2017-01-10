using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Docller.Core.Common;
using Docller.Core.DB;
using Docller.Core.Infrastructure;
using Docller.Core.Models;
using Docller.Core.Repository.Accessors;
using Docller.Core.Repository.Collections;
using Docller.Core.Repository.Mappers;
using Docller.Core.Repository.Mappers.StoredProcMappers;
using Microsoft.Practices.EnterpriseLibrary.Data;


namespace Docller.Core.Repository
{
    public class TransmittalRepository : BaseRepository, ITransmittalRepository
    {
        public TransmittalRepository(FederationType federation, long federationKey)
            : base(federation, federationKey)
        {
        }
        #region Implementation of ITransmittalRepository

        public CreateTransmittalInfo CreateTransmittal(Transmittal transmittal)
        {
            Database db = this.GetDb();
            
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            CreateTransmittalMapper createTransmittalMapper = new CreateTransmittalMapper();
            StoredProcAccessor<CreateTransmittalInfo> accessor = db.CreateStoredProcAccessor(StoredProcs.CreateTransmittal,
                                                                              parameterMapper, createTransmittalMapper);
            CreateTransmittalInfo createTransmittalInfo = accessor.ExecuteSingle(
                transmittal.CustomerId,
                transmittal.TransmittalId,
                transmittal.ProjectId,
                transmittal.TransmittalNumber,
                transmittal.Subject,
                transmittal.Message,
                transmittal.IsDraft,
                transmittal.TransmittalStatus != null
                    ? transmittal.TransmittalStatus.StatusId
                    : 0,
                transmittal.CreatedBy.UserName,
                PermissionFlag.DefaultFlag,
                new TransmittedFileCollection(transmittal.Files),
                new TransmittalUserCollection(transmittal.Distribution));

            if (parameterMapper.ReturnValue.HasValue)
            {
                createTransmittalInfo.Status = (RepositoryStatus)parameterMapper.ReturnValue.Value;
            }
            return createTransmittalInfo;
        }

        public Transmittal GetTransmittal(long projectId, long transmittalId)
        {
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            TransmittalMapper transmittalMapper = new TransmittalMapper();
            StoredProcAccessor<Transmittal> accessor =
                db.CreateStoredProcAccessor(StoredProcs.GetTransmittal, parameterMapper, transmittalMapper);

            return accessor.ExecuteSingle(projectId, transmittalId);
        }

        public IssueSheet GetIssueSheet(string usernName, long transmittalId)
        {
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            IssueSheetMapper issueSheetMapper = new IssueSheetMapper();
            StoredProcAccessor<IssueSheet> accessor = db.CreateStoredProcAccessor(StoredProcs.GetIssueSheet, parameterMapper,
                                                                    issueSheetMapper);
            return accessor.ExecuteSingle(usernName, transmittalId);
        }

        public IEnumerable<File> ShareFiles(long customerId, long transmittalId, long projectId, string message, string userName, long[] fileIds,
            string[] emails)
        {
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            StoredProcAccessor<File> accessor =
                db.CreateStoredProcAccessor<File>(StoredProcs.ShareFiles, parameterMapper,DefaultMappers.ForSharedFiles);

            FileCollection files = new FileCollection();
            files.AddRange(fileIds.Select(fileId => new File() {FileId = fileId}));
            List<User> users = new List<User>();
            users.AddRange(emails.Select(e => new User() {Email = e}));

            return accessor.Execute(customerId, transmittalId, projectId, message, userName, files,
                new UserCollection(users));
        }

        public Transmittal GetSharedFiles(long projectId, long transmittalId, string downloadedBy)
        {
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            DownloadSharedFilesMapper transmittalMapper = new DownloadSharedFilesMapper();
            StoredProcAccessor<Transmittal> accessor =
                db.CreateStoredProcAccessor(StoredProcs.DownloadSharedFiles, parameterMapper, transmittalMapper);

            return accessor.ExecuteSingle(projectId, transmittalId, downloadedBy);
        }

        public PageableData<Transmittal> GetMyTransmittals(string userName, long projectId, bool showDraft, int pageNumber, int pageSize)
        {

            return GetMyTransmittals(userName, projectId, showDraft, true, false, false, pageNumber, pageSize,
                DefaultMappers.ForMyTransmittals);
        }

        public PageableData<Transmittal> GetTransmittalsForMe(string userName, long projectId, int pageNumber, int pageSize)
        {
            return GetMyTransmittals(userName, projectId, false, false, true, false, pageNumber, pageSize,
                DefaultMappers.ForTransmittalsForMe);
        }

        public PageableData<Transmittal> GetTransmittalsForMyCompany(string userName, long projectId, int pageNumber, int pageSize)
        {
            return GetMyTransmittals(userName, projectId, false, false, false, true, pageNumber, pageSize,
                DefaultMappers.ForTransmittalsForMe);
        }

        private PageableData<Transmittal> GetMyTransmittals(string userName, long projectId, bool showDraft,
            bool createdByMe, bool sendToMe, bool sendToMyCompany, int pageNumber, int pageSize, IRowMapper<Transmittal> mapper)
        {
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            StoredProcAccessor<Transmittal> accessor =
                db.CreateStoredProcAccessor(StoredProcs.GetMyTransmittals, parameterMapper, mapper);

            IEnumerable<Transmittal> transmittals = accessor.Execute(userName, projectId, showDraft, createdByMe, sendToMe, sendToMyCompany, pageNumber,
                pageSize);

            return new PageableData<Transmittal>(transmittals);

        }

        #endregion
    }
}