CREATE PROCEDURE [dbo].[usp_SetFilePreviews]
	@FileId bigint,
	@generatedTime datetime
	
AS
	UPDATE Files
	SET PreviewsGenerated = @generatedTime
	WHERE FileID = @FileId

RETURN 0
