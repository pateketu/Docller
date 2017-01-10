CREATE TYPE [dbo].[FileTableType] AS TABLE 
(
	[FileId] bigint NULL,
	[FileInternalName] uniqueidentifier NULL,
	[FileName] [nvarchar](255) NULL,
	[FileSize] decimal NULL,
	[ProjectId] [bigint] NULL,
	[FolderId] [bigint] NULL,
	[DocNumber] [nvarchar](1000) NULL,
	[Revision] [nvarchar](10) NULL,
	[Title] nvarchar(1000) NULL,
	[Notes] nvarchar(3000) NULL,
	[ParentFile] uniqueidentifier NULL,
	[BaseFileName] [nvarchar](255) NULL,
	[IsExistingFile] bit NULL,
	[RevisionNumber] int NULL,
	[StatusId] bigint NULL
)