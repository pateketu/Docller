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
