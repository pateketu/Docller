CREATE PROCEDURE [dbo].[usp_GetFileVersionInfo]
	@FileId bigint = 0,
	@RevisionNumber int,
	@UserName nvarchar(400)
AS
	DECLARE @Files as FileInfoTableType
	DECLARE @FilesTable as FileTableType
	
	INSERT INTO @FilesTable(FileId)
	VALUES(@FileId)

	INSERT INTO @Files(FileId,FileInternalName,FileName,FileExtension,ContentType,FileSize, RevisionNumber, FullPath,FolderName,BlobContainer)
	exec usp_GetFilesInfo @UserName, @FilesTable
	--also return Permission masks
	IF EXISTS (SELECT Top 1 * FROM @Files)
	BEGIN
		DECLARE @FileVersions as FileInfoTableType
		
		INSERT INTO @FileVersions (FileId,FileInternalName,FileName,FileExtension,ContentType,FileSize, RevisionNumber, FullPath,FolderName,BlobContainer, VersionPath)	
		SELECT FV.FileId,F.FileInternalName,FV.FileName,FV.FileExtension,FV.ContentType,
			  FV.FileSize, FV.RevisionNumber, FullPath,FolderName,BlobContainer, FV.VersionPath
		FROM @Files AS F JOIN  FileVersions as FV ON F.FileId = FV.FileId 
		WHERE FV.RevisionNumber = @RevisionNumber

		SELECT FileId,FileInternalName,FileName,FileExtension,ContentType,FileSize, RevisionNumber, FullPath,FolderName,BlobContainer, VersionPath
		FROM @FileVersions
		-- Get Attachments
		SELECT FA.FileId, FA.FileName, FA.FileExtension, 
				FA.ContentType, FA.FileSize, FA.RevisionNumber, FA.VersionPath
		FROM FileAttachments AS FA 
			JOIN @FileVersions AS FV ON FA.FileId = FV.FileId 
				AND FA.RevisionNumber = FV.RevisionNumber
	END
RETURN 0
