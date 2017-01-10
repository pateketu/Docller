CREATE PROCEDURE [dbo].[usp_DeleteAttachment]
	@UserName nvarchar(400),
	@FileId bigint,
	@RevisionNumber int = 0
AS
	-- do some security validation	
	IF @RevisionNumber = 0
	BEGIN
		DELETE fa
		FROM FileAttachments as fa
		WHERE fa.FileId= @FileId
	END
	ELSE
	BEGIN
		DELETE fa
		FROM FileAttachments as fa
		WHERE fa.FileId= @FileId AND fa.RevisionNumber = @RevisionNumber
	END
	
	--Return details of the files
	exec usp_GetAttachment @UserName, @FileId
	
RETURN 0
