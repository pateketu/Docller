CREATE TABLE [dbo].[FileAttachments]
(
	[FileId] bigint NOT NULL,
	[CustomerId] [bigint] NOT NULL  CHECK(CustomerId > 0),
	[VersionPath] nvarchar(50) NULL,
	[RevisionNumber] int NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[FileExtension] [nvarchar](10) NOT NULL,
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
