CREATE TYPE [dbo].[FileInfoTableType] AS TABLE 
	(
		FileId bigint,
		FileInternalName uniqueidentifier,
		[FileName] nvarchar(255) NOT NULL,
		[FileExtension] [nvarchar](4)  NOT NULL,
		[ContentType] [nvarchar] (255) NOT NULL,
		[FileSize] int NOT NULL,
		[RevisionNumber] int NOT NULL,
		[FullPath] [nvarchar](2000) NOT NULL,
		[FolderName] [nvarchar](255) NOT NULL,
		[BlobContainer] [nvarchar](25) NOT NULL,
		[VersionPath] nvarchar(50) NULL
	)