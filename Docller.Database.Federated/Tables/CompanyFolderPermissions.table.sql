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