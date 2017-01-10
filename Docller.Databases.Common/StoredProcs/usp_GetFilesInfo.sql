CREATE PROCEDURE [dbo].[usp_GetFilesInfo]
	@UserName nvarchar(400),
	@FilesTable FileTableType READONLY
AS
	DECLARE @CompanyId bigint
	DECLARE @UserId bigint
	DECLARE @Files as FileInfoTableType
	
	
	exec @UserId = usp_GetTenantUserId @UserName
	exec @CompanyId = usp_GetUserCompanyId @UserName

	-- For security trim
	INSERT INTO @Files(FileId,FileInternalName,FileName,FileExtension,ContentType,FileSize, RevisionNumber, FullPath,FolderName,BlobContainer)
	SELECT F.FileId, F.FileInternalName, F.FileName, F.FileExtension, F.ContentType, F.FileSize, F.CurrentRevision, folder.FullPath,folder.FolderName,P.BlobContainer
	FROM Files as F 
	JOIN @FilesTable AS FT ON F.FileId = FT.FileId
	JOIN Folders as folder ON F.FolderId = folder.FolderId
	JOIN Projects AS P ON F.ProjectId = P.ProjectId
	
	SELECT FileId,FileInternalName,FileName,FileExtension,ContentType,FileSize, RevisionNumber, FullPath,FolderName,BlobContainer
	FROM @Files

	

RETURN 0
