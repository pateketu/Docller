using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Docller.Core.Common;
using Docller.Core.Models;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Mappers
{
    internal static class DefaultMappers
    {

        /// <summary>
        /// Gets for customer.
        /// </summary>
        public static IRowMapper<Customer> ForCustomer
        {
            get
            {
                return MapBuilder<Customer>.MapAllProperties()
                    .DoNotMap(x => x.AdminUser)
                    .Build();
            }
        }

        public static IRowMapper<Folder> ForDuplicateFolders
        {
            get
            {
                return MapBuilder<Folder>.MapNoProperties()
                    .MapByName(x => x.FolderName)
                    .Build();
            }
        }

        public static IRowMapper<Company> ForFilePreferences
        {
            get
            {
                return MapBuilder<Company>.MapNoProperties()
                    .MapByName(x => x.RevisionRegEx)
                    .MapByName(x => x.AttributeMappingsXml)
                    .Build();
            }
        }

        public static IRowMapper<File> ForSharedFiles
        {
            get
            {
                return MapBuilder<File>.MapNoProperties()
                    .MapByName(x => x.FileId)
                    .MapByName(x => x.FileName)
                    .MapByName(x => x.FileExtension)
                    .MapByName(x => x.FileInternalName)
                    .MapByName(x => x.FileSize)
                    .Build();
            }
        }

        public static IRowMapper<Project> ForProjectDetails
        {
            get
            {
                return MapBuilder<Project>.MapNoProperties()
                    .MapByName(x => x.ProjectId)
                    .MapByName(x => x.ProjectName)
                    .MapByName(x => x.ProjectImage)
                    .MapByName(x => x.BlobContainer)
                    .Build();
            }
        }

        public static IRowMapper<Status> ForProjectStatus
        {
            get
            {
                return MapBuilder<Status>.MapNoProperties()
                    .MapByName(x => x.StatusId)
                    .MapByName(x => x.StatusText)
                    .MapByName(x => x.StatusLongText)
                    .Build();
            }
        }


        public static IRowMapper<File> ForUpdateFiles
        {
            get
            {
                return MapBuilder<File>.MapNoProperties()
                    .MapByName(x => x.FileId)
                    .MapByName(x => x.FileInternalName)
                    .MapByName(x => x.FileName)
                    .MapByName(x => x.IsExistingFile)
                    .Build();
            }
        }

        public static IRowMapper<File> ForFilesToEdit
        {
            get
            {
                return MapBuilder<File>.MapNoProperties()
                    .MapByName(x => x.FileId)
                    .MapByName(x => x.FileInternalName)
                    .MapByName(x => x.FileName)
                    .MapByName(x => x.BaseFileName)
                    .MapByName(x => x.FileExtension)
                    .MapByName(x => x.DocNumber)
                    .MapByName(x => x.Revision)
                    .MapByName(x => x.Title)
                    .MapByName(x => x.Notes)
                    .MapByName(x => x.ModifiedDate)
                    .MapByName(x => x.CreatedDate)
                    .Map(x => x.Project).WithFunc(reader => new Project() {ProjectId = reader.GetInt64(12)})
                    .Map(x => x.Folder).WithFunc(reader => new Folder() {FolderId = reader.GetInt64(13)})
                    .Map(x => x.ModifiedBy).WithFunc(
                        reader =>
                            new User()
                            {
                                FirstName = reader.GetNullableString(14),
                                LastName = reader.GetNullableString(15),
                                UserName = reader.GetNullableString(16),
                                Email = reader.GetNullableString(17)
                            })
                    .MapByName(x => x.StatusId)
                    .Map(x => x.Status).ToColumn("StatusText")
                    .Map(x => x.Attachments).WithFunc(reader => reader.IsDBNull(21)
                        ? null
                        : new List<FileAttachment>()
                        {
                            new FileAttachment()
                            {
                                FileName =
                                    reader.GetString(21)
                            }
                        })
                    .Build();

            }
        }

        public static IRowMapper<File> ForFileDownload
        {
            get
            {
                return MapBuilder<File>.MapNoProperties()
                    .MapByName(x => x.FileId)
                    .MapByName(x => x.FileInternalName)
                    .MapByName(x => x.FileName)
                    .MapByName(x => x.FileExtension)
                    .MapByName(x => x.ContentType)
                    .MapByName(x => x.FileSize)
                    .Map(x => x.Folder)
                    .WithFunc(
                        r => new Folder() {FullPath = r.GetString(7), FolderName = r.GetString(8)})
                    .Map(x => x.Project)
                    .WithFunc(r => new Project() {BlobContainer = r.GetString(9)})
                    .Build();

            }

        }
        public static IRowMapper<MarkUpFile> ForMarkUp
        {
            get
            {
                return MapBuilder<MarkUpFile>.MapNoProperties()
                    .MapByName(x => x.FileId)
                    .Map(x => x.ParentFile).ToColumn("FileInternalName")
                    .MapByName(x => x.FileName)
                    .Map(x => x.FileRevisionNumber).ToColumn("RevisionNumber")
                    .MapByName(x => x.FileExtension)
                    .MapByName(x => x.ContentType)
                    .MapByName(x => x.FileSize)
                    .Map(x => x.Folder)
                    .WithFunc(
                        r => new Folder() { FullPath = r.GetString(7), FolderName = r.GetString(8) })
                    .Map(x => x.Project)
                    .WithFunc(r => new Project() { BlobContainer = r.GetString(9) })
                    .Build();

            }

        }

        public static IRowMapper<FileVersion> ForFileVersionDownload
        {
            get
            {
                return MapBuilder<FileVersion>.MapNoProperties()
                    .MapByName(x => x.FileId)
                    .MapByName(x => x.FileInternalName)
                    .MapByName(x => x.FileName)
                    .MapByName(x => x.FileExtension)
                    .MapByName(x => x.ContentType)
                    .MapByName(x => x.FileSize)
                    .MapByName(x => x.VersionPath)
                    .Map(x => x.Folder)
                    .WithFunc(
                        r => new Folder() {FullPath = r.GetString(7), FolderName = r.GetString(8)})
                    .Map(x => x.Project)
                    .WithFunc(r => new Project() {BlobContainer = r.GetString(9)})
                    .Build();

            }

        }

        public static IRowMapper<User> ForUserLogOnInfo
        {
            get
            {
                return MapBuilder<User>.MapNoProperties()
                    .MapByName(x => x.FirstName)
                    .MapByName(x => x.LastName)
                    .MapByName(x => x.IsCustomerAdmin)
                    .MapByName(x => x.FailedLogInAttempt)
                    .MapByName(x => x.IsLocked)
                    .Build();
            }
        }

        public static IRowMapper<User> ForUserSubscriptionInfo
        {
            get
            {
                return MapBuilder<User>.MapNoProperties()
                    .MapByName(x => x.FirstName)
                    .MapByName(x => x.LastName)
                    .MapByName(x => x.IsCustomerAdmin)
                    .MapByName(x => x.FailedLogInAttempt)
                    .MapByName(x => x.IsLocked)
                    .Build();
            }
        }

        public static IRowMapper<FileAttachment> ForFileAttachment
        {
            get
            {
                IRowMapper<FileAttachment> mapper =
                    MapBuilder<FileAttachment>.MapNoProperties()
                        .MapByName(x => x.FileId)
                        .MapByName(x => x.ParentFile)
                        .MapByName(x => x.RevisionNumber)
                        .MapByName(x => x.FileName)
                        .MapByName(x => x.FileExtension)
                        .MapByName(x => x.ContentType)
                        .MapByName(x => x.FileSize)
                        .MapByName(x => x.CreatedDate)
                        .MapByName(x => x.RevisionNumber)
                        .Map(x => x.Folder).WithFunc(x => new Folder() {FullPath = x.GetString(9)})
                        .Map(x => x.Project)
                        .WithFunc(x => new Project() {BlobContainer = x.GetString(10)})
                        .Map(x => x.CreatedBy)
                        .WithFunc(
                            x =>
                                new User()
                                {
                                    FirstName = x.GetNullableString(11),
                                    LastName = x.GetNullableString(12)
                                })
                        .Build();
                return mapper;
            }
        }

        public static IRowMapper<PermissionInfo> ForProjectPermissions
        {
            get
            {
                return
                    MapBuilder<PermissionInfo>.MapNoProperties()
                        .MapByName(x => x.EntityId)
                        .MapByName(x => x.FirstName)
                        .MapByName(x => x.LastName)
                        .MapByName(x => x.Email)
                        .MapByName(x => x.CompanyName)
                        .Map(x => x.Permissions).WithFunc((x => (PermissionFlag) x.GetInt32(5)))
                        .Build();
            }
        }

        public static IRowMapper<PermissionInfo> ForFolderPermissions
        {
            get
            {
                return
                    MapBuilder<PermissionInfo>.MapNoProperties()
                        .MapByName(x => x.EntityId)
                        .MapByName(x => x.CompanyName)
                        .Map(x => x.Permissions).WithFunc((x => (PermissionFlag) x.GetInt32(2)))
                        .Build();
            }
        }

        public static IRowMapper<Transmittal> ForMyTransmittals
        {
            get
            {
                return
                    MapBuilder<Transmittal>.MapNoProperties()
                        .MapByName(x => x.TransmittalId)
                        .MapByName(x => x.TransmittalNumber)
                        .MapByName(x => x.Subject)
                        .MapByName(x => x.Message)
                        .MapByName(x => x.CreatedDate)
                        .Map(x => x.TransmittalStatus)
                        .WithFunc(
                            r =>
                                new Status()
                                {
                                    StatusText = r.GetNullableString(5),
                                    StatusLongText = r.GetNullableString(6)
                                })
                        .Build();
            }

        }

        public static IRowMapper<Transmittal> ForTransmittalsForMe
        {
            get
            {
                return
                    MapBuilder<Transmittal>.MapNoProperties()
                        .MapByName(x => x.TransmittalId)
                        .MapByName(x => x.TransmittalNumber)
                        .MapByName(x => x.Subject)
                        .MapByName(x => x.Message)
                        .MapByName(x => x.CreatedDate)
                        .Map(x => x.TransmittalStatus)
                        .WithFunc(
                            r =>
                                new Status()
                                {
                                    StatusText = r.GetNullableString(5),
                                    StatusLongText = r.GetNullableString(6)
                                })
                        .Map(x => x.Distribution)
                        .WithFunc(r => new List<TransmittalUser>() {new TransmittalUser() {IsCced = r.GetBoolean(7)}})
                        .Build();
            }
        }
    }
}
