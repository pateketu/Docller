CREATE PROCEDURE [dbo].[usp_GetAttachment]
	@UserName nvarchar(400),
	@FileId bigint	
AS
	-- need to do some security trimming

	SELECT fa.FileId, f.FileInternalName as ParentFile, fa.RevisionNumber, 
	fa.FileName, fa.FileExtension, fa.ContentType, fa.FileSize, fa.CreatedDate, fa.RevisionNumber, folder.FullPath, p.BlobContainer, u.FirstName, u.LastName
	FROM FileAttachments as fa JOIN Users as u on fa.CreatedBy = u.UserId
	JOIN Files as f on fa.FileId = f.FileId and fa.RevisionNumber = f.CurrentRevision
	JOIN Folders as folder on fa.FolderId = folder.FolderId
	JOIN Projects as p on p.ProjectId = fa.ProjectId	
	WHERE f.FileId = @FileId

	SELECT DISTINCT fa.FileId, fv.RevisionNumber, fv.VersionPath, fv.FileName, fv.FileExtension, fv.ContentType, fv.CreatedDate, u.FirstName, u.LastName  
	FROM FileAttachments as fa 
	JOIN FileAttachments as fv on fa.FileId = fv.FileId
	JOIN Users as u on fa.CreatedBy = u.UserId
	WHERE fa.FileId = @FileId
	ORDER BY fv.RevisionNumber asc 


RETURN 0
