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
	[ModifiedBy] [int] NOT NULL
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
