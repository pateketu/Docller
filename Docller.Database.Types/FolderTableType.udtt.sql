CREATE TYPE [dbo].[FolderTableType] AS TABLE 
(
	[FolderId] [bigint] NULL,
	[FolderName] [nvarchar](255) NULL,
	[ParentFolderId] [int] NULL,
	[FullPath] [nvarchar](2000) NULL
)