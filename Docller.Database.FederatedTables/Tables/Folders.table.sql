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