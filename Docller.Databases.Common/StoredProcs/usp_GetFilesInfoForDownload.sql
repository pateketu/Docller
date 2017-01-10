CREATE PROCEDURE [dbo].[usp_GetFilesInfoForDownload]
	@UserName nvarchar(400),
	@FilesTable FileTableType READONLY
AS
	
	DECLARE @Files as FileInfoTableType
	
	INSERT INTO @Files(FileId,FileInternalName,FileName,FileExtension,ContentType,FileSize, RevisionNumber, FullPath,FolderName,BlobContainer)
	exec usp_GetFilesInfo @UserName, @FilesTable
	
	SELECT FileId,FileInternalName,FileName,FileExtension,ContentType,FileSize, RevisionNumber, FullPath,FolderName,BlobContainer
	FROM @Files

		
	-- Get Attachments
	SELECT FA.FileId, FA.FileName, FA.FileExtension, 
			FA.ContentType, FA.FileSize, FA.RevisionNumber, FA.VersionPath
	FROM FileAttachments AS FA 
		JOIN @Files AS F ON FA.FileId = F.FileId 
			AND FA.RevisionNumber = F.RevisionNumber



RETURN 0
