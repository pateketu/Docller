USE FEDERATION ROOT WITH RESET
GO
CREATE FEDERATION Customer_Federation(cid BIGINT RANGE)
GO
USE FEDERATION Customer_Federation(cid = 0) WITH RESET, FILTERING=OFF
GO
PRINT N'Creating [dbo].[Companies]...';


GO
CREATE TABLE [dbo].[Companies](
	[CompanyId] [bigint] NOT NULL,
	[CustomerId] BIGINT NOT NULL CHECK(CustomerId > 0),
	[CompanyName] [nvarchar](255) NOT NULL,
	[CompanyAlias] [nvarchar](5) NULL,
	[CompanyNameHash]  AS (CONVERT([varbinary](20),hashbytes('SHA1',lower([CompanyName])),(0))) PERSISTED NOT NULL,
	[RevisionRegEx] [nvarchar](50) NULL,
	[AttributeMappingsXml] [nvarchar](3000) NULL,
	[CreatedDate] datetime default(getdate()) NOT NULL	
	PRIMARY KEY (CompanyId, CustomerId) 
)FEDERATED ON (cid=CustomerId)


GO
PRINT N'Creating [dbo].[Companies].[IX_CompanyNameHash]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_CompanyNameHash]
    ON [dbo].[Companies]([CompanyNameHash] ASC, [CustomerId] ASC);


GO
PRINT N'Creating [dbo].[CompanyFolderPermissions]...';


GO
CREATE TABLE [dbo].[CompanyFolderPermissions]
(
	[FolderId] [bigint] NOT NULL,
	[CompanyId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL  CHECK(CustomerId > 0),
	[PermissionMask] [int] NOT NULL,
	[CreatedDate] datetime default(getdate()) NOT NULL 
	PRIMARY KEY (FolderId, CompanyId, CustomerId),
	CONSTRAINT FK_CompanyFolderPermissions_Folders FOREIGN KEY(FolderId, CustomerId) 
		 REFERENCES Folders(FolderId, CustomerId),
	CONSTRAINT FK_CompanyFolderPermissions_Companies FOREIGN KEY(CompanyId, CustomerId) 
	REFERENCES Companies(CompanyId, CustomerId)
)FEDERATED ON (cid=CustomerId)


GO
PRINT N'Creating [dbo].[CompanyUsers]...';


GO
CREATE TABLE CompanyUsers
(
	CompanyId BIGINT NOT NULL,
	CustomerId BIGINT NOT NULL CHECK(CustomerId > 0),
	UserId INT NOT NULL,
	--	CompanyName NVARCHAR(255) NOT NULL,
	[CreatedDate] datetime default(getdate()) NOT NULL 
	PRIMARY KEY (CompanyId, CustomerId, UserId),
	CONSTRAINT FK_Companies_CompanyUsers FOREIGN KEY(CompanyId, CustomerId) 
		 REFERENCES Companies(CompanyId, CustomerId)
)FEDERATED ON (cid=CustomerId)


GO
PRINT N'Creating [dbo].[FileAttachments]...';


GO
CREATE TABLE [dbo].[FileAttachments]
(
	[FileId] bigint NOT NULL,
	[CustomerId] [bigint] NOT NULL  CHECK(CustomerId > 0),
	[VersionPath] nvarchar(50) NULL,
	[RevisionNumber] int NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[FileExtension] [nvarchar](4) NOT NULL,
	[ContentType] [nvarchar] (255) NOT NULL,
	[BaseFileName] [nvarchar](255) NOT NULL,
	[FileNameHash]  AS (CONVERT([varbinary](20),hashbytes('SHA1',lower([BaseFileName])),(0))) PERSISTED NOT NULL,
	[FileSize] decimal NOT NULL,
	[ProjectId] [bigint] NOT NULL,
	[FolderId] [bigint] NOT NULL,
	[CreatedDate] datetime default(getdate()) NOT NULL,
	[ModifiedDate] datetime default(getdate()) NOT NULL,
	[CreatedBy] int NOT NULL,
	[ModifiedBy] [int] NOT NULL
	PRIMARY KEY (FileId,RevisionNumber, CustomerId)
	CONSTRAINT FK_Files_FileAttachments FOREIGN KEY(FileId, CustomerId) 
		 REFERENCES Files(FileId, CustomerId),
	CONSTRAINT FK_FileAttachments_Projects FOREIGN KEY(ProjectId, CustomerId) 
		 REFERENCES Projects(ProjectId, CustomerId),
	CONSTRAINT FK_FileAttachments_Folders FOREIGN KEY(FolderId, CustomerId) 
		REFERENCES Folders(FolderId, CustomerId),
	CONSTRAINT FK_FileAttachments_CreatedBy FOREIGN KEY(CreatedBy, CustomerId) 
		REFERENCES UserCache(UserId, CustomerId),
	CONSTRAINT FK_FileAttachments_ModifiedBy FOREIGN KEY(ModifiedBy, CustomerId) 
		REFERENCES UserCache(UserId, CustomerId)
)FEDERATED ON (cid=CustomerId)


GO
PRINT N'Creating [dbo].[FileAttachments].[IX_FileAttachmentsClustered]...';


GO
CREATE NONCLUSTERED INDEX [IX_FileAttachmentsClustered]
    ON [dbo].[FileAttachments]([FileId] ASC, [CustomerId] ASC);


GO
PRINT N'Creating [dbo].[Files]...';


GO
CREATE TABLE [dbo].[Files]
(
	[FileId] bigint NOT NULL,
	[FileInternalName] uniqueidentifier NOT NULL,
	[CustomerId] [bigint] NOT NULL  CHECK(CustomerId > 0),
	[FileName] [nvarchar](255) NOT NULL,
	[FileExtension] [nvarchar](4) NOT NULL,
	[ContentType] [nvarchar] (255) NOT NULL,
	[BaseFileName] [nvarchar](255) NOT NULL,
	[FileNameHash]  AS (CONVERT([varbinary](20),hashbytes('SHA1',lower([BaseFileName])),(0))) PERSISTED NOT NULL,
	[CurrentRevision] int DEFAULT(1)  NOT NULL,
	[FileSize] int NOT NULL,
	[DocNumber] [nvarchar](255) NULL,
	[Revision] [nvarchar](10) NULL,
	[StatusId] bigint NULL,
	[Title] nvarchar(1000) NULL,
	[Notes] nvarchar(3000) NULL,
	[ThumbnailUrl] nvarchar(1000) NULL,
	[PreviewUrl] nvarchar(1000) NULL,
	[ProjectId] [bigint] NOT NULL,
	[FolderId] [bigint] NOT NULL,
	[CreatedDate] datetime default(getdate()) NOT NULL,
	[ModifiedDate] datetime default(getdate()) NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[PreviewsGenerated] datetime NULL 
	PRIMARY KEY (FileId, CustomerId),
	CONSTRAINT FK_Files_Projects FOREIGN KEY(ProjectId, CustomerId) 
		 REFERENCES Projects(ProjectId, CustomerId),
	CONSTRAINT FK_Files_Folders FOREIGN KEY(FolderId, CustomerId) 
		REFERENCES Folders(FolderId, CustomerId),
	CONSTRAINT FK_Files_Status FOREIGN KEY(StatusId, CustomerId) 
		REFERENCES Status(StatusId, CustomerId),
	CONSTRAINT FK_Files_CreatedBy FOREIGN KEY(CreatedBy, CustomerId) 
		REFERENCES UserCache(UserId, CustomerId),
	CONSTRAINT FK_Files_ModifiedBy FOREIGN KEY(ModifiedBy, CustomerId) 
		REFERENCES UserCache(UserId, CustomerId)
)FEDERATED ON (cid=CustomerId)



GO
PRINT N'Creating [dbo].[FileVersions]...';


GO
CREATE TABLE [dbo].[FileVersions]
(
	[FileId] bigint NOT NULL,
	[RevisionNumber] int NOT NULL   CHECK(RevisionNumber > 0),
	[CustomerId] [bigint] NOT NULL  CHECK(CustomerId > 0),
	[VersionPath] nvarchar(50) NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[FileExtension] [nvarchar](4) NOT NULL,
	[ContentType] [nvarchar] (255) NOT NULL,
	[FileSize] decimal NOT NULL,
	[DocNumber] [nvarchar](255) NULL,
	[Revision] [nvarchar](10) NULL,
	[StatusId] bigint NULL,
	[Title] nvarchar(1000) NULL,
	[Notes] nvarchar(3000) NULL,
	[ProjectId] [bigint] NOT NULL,
	[FolderId] [bigint] NOT NULL,
	[CreatedDate] datetime default(getdate()) NOT NULL,
	[ModifiedDate] datetime default(getdate()) NOT NULL,
	[CreatedBy] int NOT NULL,
	[ModifiedBy] [int] NOT NULL
	PRIMARY KEY (FileId,RevisionNumber, CustomerId)
	CONSTRAINT FK_FileVersions_File FOREIGN KEY(FileId, CustomerId) 

		 REFERENCES Files(FileId, CustomerId),
	CONSTRAINT FK_FileVersions_Projects FOREIGN KEY(ProjectId, CustomerId) 
		 REFERENCES Projects(ProjectId, CustomerId),
	CONSTRAINT FK_FileVersions_Folders FOREIGN KEY(FolderId, CustomerId) 
		REFERENCES Folders(FolderId, CustomerId),
	CONSTRAINT FK_FileVersions_Status FOREIGN KEY(StatusId, CustomerId) 
		REFERENCES Status(StatusId, CustomerId),
	CONSTRAINT FK_FileVersions_CreatedBy FOREIGN KEY(CreatedBy, CustomerId) 
		REFERENCES UserCache(UserId, CustomerId),
	CONSTRAINT FK_FileVersions_ModifiedBy FOREIGN KEY(ModifiedBy, CustomerId) 
		REFERENCES UserCache(UserId, CustomerId)
)FEDERATED ON (cid=CustomerId)



GO
PRINT N'Creating [dbo].[Folders]...';


GO
CREATE TABLE [dbo].[Folders](
	[FolderId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL  CHECK(CustomerId > 0),	
	[ProjectId] [bigint] NOT NULL,
	[FolderName] [nvarchar](255) NOT NULL,
	[FolderNameHash]  AS (CONVERT([varbinary](20),hashbytes('SHA1',lower([FolderName])),(0))) PERSISTED NOT NULL,
	[FolderInternalName] AS (CONVERT([nvarchar](350), 'f-' + CONVERT([nvarchar](20), FolderId))) PERSISTED NOT NULL,	
	[ParentFolderId] [bigint] NULL,
	[FullPath] [nvarchar](2000) NULL,
	[CreatedDate] datetime default(getdate()) NOT NULL, 
	[ModifiedDate] datetime default(getdate()) NOT NULL
	PRIMARY KEY (FolderId, CustomerId),
	CONSTRAINT FK_Folders_Projects FOREIGN KEY(ProjectId, CustomerId) 
		 REFERENCES Projects(ProjectId, CustomerId),
	CONSTRAINT FK_Folders_ParentFolders FOREIGN KEY(ParentFolderId, CustomerId) 
			REFERENCES Folders(FolderId, CustomerId)
)FEDERATED ON (cid=CustomerId)


GO
PRINT N'Creating [dbo].[Projects]...';


GO
CREATE TABLE [dbo].[Projects](
	[ProjectId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL  CHECK(CustomerId > 0),
	[ProjectName] [nvarchar](255) NOT NULL,
	[ProjectNameHash]  AS (CONVERT([varbinary](20),hashbytes('SHA1',lower([ProjectName])),(0))) PERSISTED NOT NULL,
	[ProjectCode] [nvarchar](10)  NULL,
	[BlobContainer] [nvarchar](255) NULL,	
	[ProjectImage] [nvarchar](255) NULL,
	[CreatedDate] datetime default(getdate()) NOT NULL 
	PRIMARY KEY (ProjectId, CustomerId) 
)FEDERATED ON (cid=CustomerId)

GO
PRINT N'Creating [dbo].[ProjectUsers]...';


GO
CREATE TABLE ProjectUsers
(
	CustomerId [bigint] NOT NULL  CHECK(CustomerId > 0),
	ProjectId Bigint NOT NULL,
	UserId INT  NOT NULL,
	PermissionMask INT NOT NULL default(0),
	[CreatedDate] datetime default(getdate()) NOT NULL 
	PRIMARY KEY (CustomerId, ProjectId,UserId),
	CONSTRAINT FK_Projects_ProjectUsers FOREIGN KEY(ProjectId, CustomerId) 
		 REFERENCES Projects(ProjectId, CustomerId)
)FEDERATED ON (cid=CustomerId)
GO

PRINT N'Creating [dbo].[QuickFileShareDistribution]...';
CREATE TABLE [dbo].[QuickFileShareDistribution]
(
	TransmittalId bigint not null,
	CustomerId bigint not null   CHECK(CustomerId > 0),
	Email nvarchar(400) not null,
	[EmailHash]  AS (CONVERT([varbinary](20),hashbytes('SHA1',lower([Email])),(0))) PERSISTED NOT NULL,
	UserId int null	
	PRIMARY KEY (TransmittalId, EmailHash, CustomerId),
	CONSTRAINT FK_QuickFileShareDistribution_Transmittals FOREIGN KEY(TransmittalId, CustomerId) 
		REFERENCES Transmittals(TransmittalId, CustomerId)
)FEDERATED ON (cid=CustomerId)

PRINT N'Creating [dbo].[Status]...';


GO
CREATE TABLE [dbo].[Status]
(
	StatusId bigint NOT NULL,
	[CustomerId] BIGINT NOT NULL CHECK(CustomerId > 0), 
	ProjectId bigint NOT NULL,
	StatusText nvarchar(200) NOT NULL,
	StatusLongText nvarchar(255) NULL
	PRIMARY KEY (StatusId, CustomerId)
	CONSTRAINT FK_Status_Projects FOREIGN KEY(ProjectId, CustomerId)
		 REFERENCES Projects(ProjectId, CustomerId),
)FEDERATED ON (cid=CustomerId)



GO
PRINT N'Creating [dbo].[TransmittalDistribution]...';


GO
CREATE TABLE [dbo].[TransmittalDistribution]
(
	TransmittalId bigint not null,
	CustomerId bigint not null   CHECK(CustomerId > 0),
	UserId int not null,
	Cced bit not null default(0)
	PRIMARY KEY (TransmittalId, UserId, CustomerId),
	CONSTRAINT FK_TransmittalDistribution_UserCache FOREIGN KEY(UserId, CustomerId) 
		 REFERENCES UserCache(UserId, CustomerId),
	CONSTRAINT FK_TransmittalDistribution_Transmittals FOREIGN KEY(TransmittalId, CustomerId) 
		REFERENCES Transmittals(TransmittalId, CustomerId)
)FEDERATED ON (cid=CustomerId)



GO
PRINT N'Creating [dbo].[Transmittals]...';


GO
CREATE TABLE [dbo].[Transmittals]
(
	TransmittalId  bigint NOT NULL,
	CustomerId bigint not null CHECK(CustomerId > 0),
	ProjectId bigint not null,
	TransmittalNumber nvarchar(50) NULL, 
	Subject nvarchar(255) null,
	Message nvarchar(2000) null,
	IsDraft bit default(0),
	IsQuickShare bit default(0),
	StatusId bigint NULL,
	CreatedDate datetime default(getutcdate()) not null,
	[CreatedBy] [int] NOT NULL	
	PRIMARY KEY (TransmittalId, CustomerId),
	CONSTRAINT FK_Transmittal_Projects FOREIGN KEY(ProjectId, CustomerId) 
		 REFERENCES Projects(ProjectId, CustomerId),
	CONSTRAINT FK_Transmittal_Status FOREIGN KEY(StatusId, CustomerId) 
		REFERENCES Status(StatusId, CustomerId),
	CONSTRAINT FK_Transmittal_CreatedBy FOREIGN KEY(CreatedBy, CustomerId) 
		REFERENCES UserCache(UserId, CustomerId),
)FEDERATED ON (cid=CustomerId)



GO
PRINT N'Creating [dbo].[TransmittedFiles]...';


GO
CREATE TABLE [dbo].[TransmittedFiles]
(
	TransmittalId bigint not null,
	CustomerId bigint not null   CHECK(CustomerId > 0),
	FileId bigint not null,
	RevisionNumber int not null
	PRIMARY KEY (TransmittalId, FileId, CustomerId, RevisionNumber),
	CONSTRAINT FK_TransmittedFiles_Files FOREIGN KEY(FileId, CustomerId) 
		 REFERENCES Files(FileId, CustomerId),
	CONSTRAINT FK_TransmittedFiles_Transmittals FOREIGN KEY(TransmittalId, CustomerId) 
		REFERENCES Transmittals(TransmittalId, CustomerId)
)FEDERATED ON (cid=CustomerId)


GO
PRINT N'Creating [dbo].[UserCache]...';


GO
CREATE TABLE [dbo].[UserCache]
(
	UserId INT NOT NULL, 
	CustomerId BIGINT NOT NULL CHECK(CustomerId > 0),
	[UserName] [nvarchar](400) NOT NULL,
	[UserNameHash]  AS (CONVERT([varbinary](20),hashbytes('SHA1',lower([UserName])),(0))) PERSISTED NOT NULL,
	[FirstName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NULL,
	[Email] [nvarchar](400) NOT NULL,
	[UserPermissions] int NOT NULL default(1)
	PRIMARY KEY (UserId, CustomerId)
	
)FEDERATED ON (cid=CustomerId)


GO
/*
PRINT N'Creating Default Constraint on [dbo].[Companies]....';


GO
ALTER TABLE [dbo].[Companies]
    ADD DEFAULT (getdate()) FOR [CreatedDate];


GO
PRINT N'Creating Default Constraint on [dbo].[CompanyFolderPermissions]....';


GO
ALTER TABLE [dbo].[CompanyFolderPermissions]
    ADD DEFAULT (getdate()) FOR [CreatedDate];


GO
PRINT N'Creating Default Constraint on [dbo].[CompanyUsers]....';


GO
ALTER TABLE [dbo].[CompanyUsers]
    ADD DEFAULT (getdate()) FOR [CreatedDate];


GO
PRINT N'Creating Default Constraint on [dbo].[FileAttachments]....';


GO
ALTER TABLE [dbo].[FileAttachments]
    ADD DEFAULT (getdate()) FOR [CreatedDate];


GO
PRINT N'Creating Default Constraint on [dbo].[FileAttachments]....';


GO
ALTER TABLE [dbo].[FileAttachments]
    ADD DEFAULT (getdate()) FOR [ModifiedDate];


GO
PRINT N'Creating Default Constraint on [dbo].[Files]....';


GO
ALTER TABLE [dbo].[Files]
    ADD DEFAULT (1) FOR [CurrentRevision];


GO
PRINT N'Creating Default Constraint on [dbo].[Files]....';


GO
ALTER TABLE [dbo].[Files]
    ADD DEFAULT (getdate()) FOR [CreatedDate];


GO
PRINT N'Creating Default Constraint on [dbo].[Files]....';


GO
ALTER TABLE [dbo].[Files]
    ADD DEFAULT (getdate()) FOR [ModifiedDate];


GO
PRINT N'Creating Default Constraint on [dbo].[FileVersions]....';


GO
ALTER TABLE [dbo].[FileVersions]
    ADD DEFAULT (getdate()) FOR [CreatedDate];


GO
PRINT N'Creating Default Constraint on [dbo].[FileVersions]....';


GO
ALTER TABLE [dbo].[FileVersions]
    ADD DEFAULT (getdate()) FOR [ModifiedDate];


GO
PRINT N'Creating Default Constraint on [dbo].[Folders]....';


GO
ALTER TABLE [dbo].[Folders]
    ADD DEFAULT (getdate()) FOR [CreatedDate];


GO
PRINT N'Creating Default Constraint on [dbo].[Folders]....';


GO
ALTER TABLE [dbo].[Folders]
    ADD DEFAULT (getdate()) FOR [ModifiedDate];


GO
PRINT N'Creating Default Constraint on [dbo].[Projects]....';


GO
ALTER TABLE [dbo].[Projects]
    ADD DEFAULT (getdate()) FOR [CreatedDate];


GO
PRINT N'Creating Default Constraint on [dbo].[ProjectUsers]....';


GO
ALTER TABLE [dbo].[ProjectUsers]
    ADD DEFAULT (0) FOR [PermissionMask];


GO
PRINT N'Creating Default Constraint on [dbo].[ProjectUsers]....';


GO
ALTER TABLE [dbo].[ProjectUsers]
    ADD DEFAULT (getdate()) FOR [CreatedDate];


GO
PRINT N'Creating Default Constraint on [dbo].[TransmittalDistribution]....';


GO
ALTER TABLE [dbo].[TransmittalDistribution]
    ADD DEFAULT (0) FOR [Cced];


GO
PRINT N'Creating Default Constraint on [dbo].[Transmittals]....';


GO
ALTER TABLE [dbo].[Transmittals]
    ADD DEFAULT (0) FOR [IsDraft];


GO
PRINT N'Creating Default Constraint on [dbo].[Transmittals]....';


GO
ALTER TABLE [dbo].[Transmittals]
    ADD DEFAULT (getutcdate()) FOR [CreatedDate];


GO
PRINT N'Creating FK_CompanyFolderPermissions_Folders...';


GO
ALTER TABLE [dbo].[CompanyFolderPermissions]
    ADD CONSTRAINT [FK_CompanyFolderPermissions_Folders] FOREIGN KEY ([FolderId], [CustomerId]) REFERENCES [dbo].[Folders] ([FolderId], [CustomerId]);


GO
PRINT N'Creating FK_CompanyFolderPermissions_Companies...';


GO
ALTER TABLE [dbo].[CompanyFolderPermissions]
    ADD CONSTRAINT [FK_CompanyFolderPermissions_Companies] FOREIGN KEY ([CompanyId], [CustomerId]) REFERENCES [dbo].[Companies] ([CompanyId], [CustomerId]);


GO
PRINT N'Creating FK_Companies_CompanyUsers...';


GO
ALTER TABLE [dbo].[CompanyUsers]
    ADD CONSTRAINT [FK_Companies_CompanyUsers] FOREIGN KEY ([CompanyId], [CustomerId]) REFERENCES [dbo].[Companies] ([CompanyId], [CustomerId]);


GO
PRINT N'Creating FK_Files_FileAttachments...';


GO
ALTER TABLE [dbo].[FileAttachments]
    ADD CONSTRAINT [FK_Files_FileAttachments] FOREIGN KEY ([FileId], [CustomerId]) REFERENCES [dbo].[Files] ([FileId], [CustomerId]);


GO
PRINT N'Creating FK_FileAttachments_Projects...';


GO
ALTER TABLE [dbo].[FileAttachments]
    ADD CONSTRAINT [FK_FileAttachments_Projects] FOREIGN KEY ([ProjectId], [CustomerId]) REFERENCES [dbo].[Projects] ([ProjectId], [CustomerId]);


GO
PRINT N'Creating FK_FileAttachments_Folders...';


GO
ALTER TABLE [dbo].[FileAttachments]
    ADD CONSTRAINT [FK_FileAttachments_Folders] FOREIGN KEY ([FolderId], [CustomerId]) REFERENCES [dbo].[Folders] ([FolderId], [CustomerId]);


GO
PRINT N'Creating FK_FileAttachments_CreatedBy...';


GO
ALTER TABLE [dbo].[FileAttachments]
    ADD CONSTRAINT [FK_FileAttachments_CreatedBy] FOREIGN KEY ([CreatedBy], [CustomerId]) REFERENCES [dbo].[UserCache] ([UserId], [CustomerId]);


GO
PRINT N'Creating FK_FileAttachments_ModifiedBy...';


GO
ALTER TABLE [dbo].[FileAttachments]
    ADD CONSTRAINT [FK_FileAttachments_ModifiedBy] FOREIGN KEY ([ModifiedBy], [CustomerId]) REFERENCES [dbo].[UserCache] ([UserId], [CustomerId]);


GO
PRINT N'Creating FK_Files_Projects...';


GO
ALTER TABLE [dbo].[Files]
    ADD CONSTRAINT [FK_Files_Projects] FOREIGN KEY ([ProjectId], [CustomerId]) REFERENCES [dbo].[Projects] ([ProjectId], [CustomerId]);


GO
PRINT N'Creating FK_Files_Folders...';


GO
ALTER TABLE [dbo].[Files]
    ADD CONSTRAINT [FK_Files_Folders] FOREIGN KEY ([FolderId], [CustomerId]) REFERENCES [dbo].[Folders] ([FolderId], [CustomerId]);


GO
PRINT N'Creating FK_Files_Status...';


GO
ALTER TABLE [dbo].[Files]
    ADD CONSTRAINT [FK_Files_Status] FOREIGN KEY ([StatusId], [CustomerId]) REFERENCES [dbo].[Status] ([StatusId], [CustomerId]);


GO
PRINT N'Creating FK_Files_CreatedBy...';


GO
ALTER TABLE [dbo].[Files]
    ADD CONSTRAINT [FK_Files_CreatedBy] FOREIGN KEY ([CreatedBy], [CustomerId]) REFERENCES [dbo].[UserCache] ([UserId], [CustomerId]);


GO
PRINT N'Creating FK_Files_ModifiedBy...';


GO
ALTER TABLE [dbo].[Files]
    ADD CONSTRAINT [FK_Files_ModifiedBy] FOREIGN KEY ([ModifiedBy], [CustomerId]) REFERENCES [dbo].[UserCache] ([UserId], [CustomerId]);


GO
PRINT N'Creating FK_FileVersions_File...';


GO
ALTER TABLE [dbo].[FileVersions]
    ADD CONSTRAINT [FK_FileVersions_File] FOREIGN KEY ([FileId], [CustomerId]) REFERENCES [dbo].[Files] ([FileId], [CustomerId]);


GO
PRINT N'Creating FK_FileVersions_Projects...';


GO
ALTER TABLE [dbo].[FileVersions]
    ADD CONSTRAINT [FK_FileVersions_Projects] FOREIGN KEY ([ProjectId], [CustomerId]) REFERENCES [dbo].[Projects] ([ProjectId], [CustomerId]);


GO
PRINT N'Creating FK_FileVersions_Folders...';


GO
ALTER TABLE [dbo].[FileVersions]
    ADD CONSTRAINT [FK_FileVersions_Folders] FOREIGN KEY ([FolderId], [CustomerId]) REFERENCES [dbo].[Folders] ([FolderId], [CustomerId]);


GO
PRINT N'Creating FK_FileVersions_Status...';


GO
ALTER TABLE [dbo].[FileVersions]
    ADD CONSTRAINT [FK_FileVersions_Status] FOREIGN KEY ([StatusId], [CustomerId]) REFERENCES [dbo].[Status] ([StatusId], [CustomerId]);


GO
PRINT N'Creating FK_FileVersions_CreatedBy...';


GO
ALTER TABLE [dbo].[FileVersions]
    ADD CONSTRAINT [FK_FileVersions_CreatedBy] FOREIGN KEY ([CreatedBy], [CustomerId]) REFERENCES [dbo].[UserCache] ([UserId], [CustomerId]);


GO
PRINT N'Creating FK_FileVersions_ModifiedBy...';


GO
ALTER TABLE [dbo].[FileVersions]
    ADD CONSTRAINT [FK_FileVersions_ModifiedBy] FOREIGN KEY ([ModifiedBy], [CustomerId]) REFERENCES [dbo].[UserCache] ([UserId], [CustomerId]);


GO
PRINT N'Creating FK_Folders_Projects...';


GO
ALTER TABLE [dbo].[Folders]
    ADD CONSTRAINT [FK_Folders_Projects] FOREIGN KEY ([ProjectId], [CustomerId]) REFERENCES [dbo].[Projects] ([ProjectId], [CustomerId]);


GO
PRINT N'Creating FK_Folders_ParentFolders...';


GO
ALTER TABLE [dbo].[Folders]
    ADD CONSTRAINT [FK_Folders_ParentFolders] FOREIGN KEY ([ParentFolderId], [CustomerId]) REFERENCES [dbo].[Folders] ([FolderId], [CustomerId]);


GO
PRINT N'Creating FK_Projects_ProjectUsers...';


GO
ALTER TABLE [dbo].[ProjectUsers]
    ADD CONSTRAINT [FK_Projects_ProjectUsers] FOREIGN KEY ([ProjectId], [CustomerId]) REFERENCES [dbo].[Projects] ([ProjectId], [CustomerId]);


GO
PRINT N'Creating FK_Status_Projects...';


GO
ALTER TABLE [dbo].[Status]
    ADD CONSTRAINT [FK_Status_Projects] FOREIGN KEY ([ProjectId], [CustomerId]) REFERENCES [dbo].[Projects] ([ProjectId], [CustomerId]);


GO
PRINT N'Creating FK_TransmittalDistribution_UserCache...';


GO
ALTER TABLE [dbo].[TransmittalDistribution]
    ADD CONSTRAINT [FK_TransmittalDistribution_UserCache] FOREIGN KEY ([UserId], [CustomerId]) REFERENCES [dbo].[UserCache] ([UserId], [CustomerId]);


GO
PRINT N'Creating FK_TransmittalDistribution_Transmittals...';


GO
ALTER TABLE [dbo].[TransmittalDistribution]
    ADD CONSTRAINT [FK_TransmittalDistribution_Transmittals] FOREIGN KEY ([TransmittalId], [CustomerId]) REFERENCES [dbo].[Transmittals] ([TransmittalId], [CustomerId]);


GO
PRINT N'Creating FK_Transmittal_Projects...';


GO
ALTER TABLE [dbo].[Transmittals]
    ADD CONSTRAINT [FK_Transmittal_Projects] FOREIGN KEY ([ProjectId], [CustomerId]) REFERENCES [dbo].[Projects] ([ProjectId], [CustomerId]);


GO
PRINT N'Creating FK_Transmittal_Status...';


GO
ALTER TABLE [dbo].[Transmittals]
    ADD CONSTRAINT [FK_Transmittal_Status] FOREIGN KEY ([StatusId], [CustomerId]) REFERENCES [dbo].[Status] ([StatusId], [CustomerId]);


GO
PRINT N'Creating FK_Transmittal_CreatedBy...';


GO
ALTER TABLE [dbo].[Transmittals]
    ADD CONSTRAINT [FK_Transmittal_CreatedBy] FOREIGN KEY ([CreatedBy], [CustomerId]) REFERENCES [dbo].[UserCache] ([UserId], [CustomerId]);


GO
PRINT N'Creating FK_TransmittedFiles_Files...';


GO
ALTER TABLE [dbo].[TransmittedFiles]
    ADD CONSTRAINT [FK_TransmittedFiles_Files] FOREIGN KEY ([FileId], [CustomerId]) REFERENCES [dbo].[Files] ([FileId], [CustomerId]);


GO
PRINT N'Creating FK_TransmittedFiles_Transmittals...';


GO
ALTER TABLE [dbo].[TransmittedFiles]
    ADD CONSTRAINT [FK_TransmittedFiles_Transmittals] FOREIGN KEY ([TransmittalId], [CustomerId]) REFERENCES [dbo].[Transmittals] ([TransmittalId], [CustomerId]);


GO
PRINT N'Creating Check Constraint on [dbo].[Companies]....';


GO
ALTER TABLE [dbo].[Companies]
    ADD CHECK (CustomerId > 0);


GO
PRINT N'Creating Check Constraint on [dbo].[CompanyFolderPermissions]....';


GO
ALTER TABLE [dbo].[CompanyFolderPermissions]
    ADD CHECK (CustomerId > 0);


GO
PRINT N'Creating Check Constraint on [dbo].[CompanyUsers]....';


GO
ALTER TABLE [dbo].[CompanyUsers]
    ADD CHECK (CustomerId > 0);


GO
PRINT N'Creating Check Constraint on [dbo].[FileAttachments]....';


GO
ALTER TABLE [dbo].[FileAttachments]
    ADD CHECK (CustomerId > 0);


GO
PRINT N'Creating Check Constraint on [dbo].[Files]....';


GO
ALTER TABLE [dbo].[Files]
    ADD CHECK (CustomerId > 0);


GO
PRINT N'Creating Check Constraint on [dbo].[FileVersions]....';


GO
ALTER TABLE [dbo].[FileVersions]
    ADD CHECK (RevisionNumber > 0);


GO
PRINT N'Creating Check Constraint on [dbo].[FileVersions]....';


GO
ALTER TABLE [dbo].[FileVersions]
    ADD CHECK (CustomerId > 0);


GO
PRINT N'Creating Check Constraint on [dbo].[Folders]....';


GO
ALTER TABLE [dbo].[Folders]
    ADD CHECK (CustomerId > 0);


GO
PRINT N'Creating Check Constraint on [dbo].[Projects]....';


GO
ALTER TABLE [dbo].[Projects]
    ADD CHECK (CustomerId > 0);


GO
PRINT N'Creating Check Constraint on [dbo].[ProjectUsers]....';


GO
ALTER TABLE [dbo].[ProjectUsers]
    ADD CHECK (CustomerId > 0);


GO
PRINT N'Creating Check Constraint on [dbo].[Status]....';


GO
ALTER TABLE [dbo].[Status]
    ADD CHECK (CustomerId > 0);


GO
PRINT N'Creating Check Constraint on [dbo].[TransmittalDistribution]....';


GO
ALTER TABLE [dbo].[TransmittalDistribution]
    ADD CHECK (CustomerId > 0);


GO
PRINT N'Creating Check Constraint on [dbo].[Transmittals]....';


GO
ALTER TABLE [dbo].[Transmittals]
    ADD CHECK (CustomerId > 0);


GO
PRINT N'Creating Check Constraint on [dbo].[TransmittedFiles]....';


GO
ALTER TABLE [dbo].[TransmittedFiles]
    ADD CHECK (CustomerId > 0);


GO
PRINT N'Creating Check Constraint on [dbo].[UserCache]....';


GO
ALTER TABLE [dbo].[UserCache]
    ADD CHECK (CustomerId > 0);


GO
*/