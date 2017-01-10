CREATE PROCEDURE [dbo].[usp_GetFilesForEdit]
	@FilesTable FileTableType READONLY
AS
	SELECT f.FileId,f.FileInternalName,f.FileName,f.BaseFileName,f.FileExtension, f.FileSize,
		   f.DocNumber,f.Revision,f.Title,f.Notes,
		   f.ModifiedDate, f.CreatedDate, f.ProjectId, f.FolderId,
		   u.FirstName, u.LastName, u.UserName, u.Email,
		   f.StatusId, s.StatusLongText,s.StatusText, fa.FileName as AttachmentName
	FROM Files as f JOIN @FilesTable FT ON f.FileInternalName = FT.FileInternalName
		 JOIN UserCache as u on U.UserId = f.CreatedBy
		 LEFT JOIN FileAttachments as fa on f.FileId = fa.FileId 
				and f.CurrentRevision = fa.RevisionNumber
		 LEFT JOIN Status as s ON f.StatusId = s.StatusId 

RETURN 0