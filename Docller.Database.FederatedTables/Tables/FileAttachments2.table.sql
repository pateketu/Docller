CREATE TABLE [dbo].[FileAttachments2]
(
	[FileAttachmentId] bigint NOT NULL,	
	[ParentFileAttachmentId] bigint NOT NULL default(0),
	[FileId] bigint NOT NULL,	
	[CustomerId] [bigint] NOT NULL  CHECK(CustomerId > 0),
	[VersionPath] nvarchar(50) NULL,
	[VersionNumber] int NOT NULL,
	[RevisionNumber] int NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[FileExtension] [nvarchar](10) NOT NULL,
	[ContentType] [nvarchar] (255) NOT NULL,
	[FileNameHash]  AS (CONVERT([varbinary](20),hashbytes('SHA1',lower([FileName])),(0))) PERSISTED NOT NULL,
	[Comments] nvarchar(2000) NULL,
	[FileSize] decimal NOT NULL,
	[ProjectId] [bigint] NOT NULL,
	[FolderId] [bigint] NOT NULL,
	[IsFromApp] bit NOT NULL default(0),	
	[AppDetails] nvarchar(255) NULL,
	[CreatedDate] datetime default(getdate()) NOT NULL,
	[ModifiedDate] datetime default(getdate()) NOT NULL,
	[CreatedBy] int NOT NULL,
	[ModifiedBy] [int] NOT NULL
	PRIMARY KEY (FileAttachmentId,CustomerId)
	CONSTRAINT FK_Files_FileAttachments2 FOREIGN KEY(FileId, CustomerId) 
		 REFERENCES Files(FileId, CustomerId),
	CONSTRAINT FK_FileAttachments2_Projects FOREIGN KEY(ProjectId, CustomerId) 
		 REFERENCES Projects(ProjectId, CustomerId),
	CONSTRAINT FK_FileAttachments2_Folders FOREIGN KEY(FolderId, CustomerId) 
		REFERENCES Folders(FolderId, CustomerId),
	CONSTRAINT FK_FileAttachments2_CreatedBy FOREIGN KEY(CreatedBy, CustomerId) 
		REFERENCES UserCache(UserId, CustomerId),
	CONSTRAINT FK_FileAttachments2_ModifiedBy FOREIGN KEY(ModifiedBy, CustomerId) 
		REFERENCES UserCache(UserId, CustomerId)
)FEDERATED ON (cid=CustomerId)
