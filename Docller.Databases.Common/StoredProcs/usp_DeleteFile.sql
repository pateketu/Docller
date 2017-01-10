CREATE PROCEDURE [dbo].[usp_DeleteFile]
	@UserName nvarchar(400),
	@FileId bigint,
	@RevisionNumber int = 0
AS
	IF @RevisionNumber > 0
	BEGIN

		--delete transmittal file
		DELETE tf
		FROM TransmittedFiles as tf
		WHERE tf.FileId = @FileId and tf.RevisionNumber = @RevisionNumber

		--delete attachments first
		DELETE fa
		FROM FileAttachments as fa
		WHERE fa.FileId= @FileId and fa.RevisionNumber = @RevisionNumber

		-- Delete the version
		DELETE fv
		FROM FileVersions as fv
		WHERE fv.FileId= @FileId and fv.RevisionNumber = @RevisionNumber


		--return the history
		exec usp_GetFileHistory @FileId
	END
	ELSE
	BEGIN
		--delete transmittal file
		DELETE tf
		FROM TransmittedFiles as tf
		WHERE tf.FileId = @FileId

		--delete attachments first
		DELETE fa
		FROM FileAttachments as fa
		WHERE fa.FileId= @FileId

		-- Delete the version
		DELETE fv
		FROM FileVersions as fv
		WHERE fv.FileId= @FileId

		--delete the file
		Delete f
		FROM Files as f
		WHERE f.FileId = @FileId

	END
RETURN 0
