CREATE PROCEDURE [dbo].[usp_GetCommentMetaDataInfo]
	@UserName nvarchar(400),
	@FileName nvarchar(255),
	@FileId bigint,
	@FolderId bigint,
	@ProjectId bigint
AS
	DECLARE @UserId bigint
	exec @UserId = usp_GetTenantUserId @UserName
	DECLARE @FilesTable as FileTableType;
	--- we need to work out if the user is upload the file first first or has it got versions
	DECLARE @AttachmentId bigint
	DECLARE @CreatedBy bigint

	DECLARE @RevisionNumber as int
	SELECT @RevisionNumber = CurrentRevision FROM Files WHERE FileId = @FileId;

	SELECT @AttachmentId = FA.FileAttachmentId, @CreatedBy = FA.CreatedBy
		FROM FileAttachments2 as FA
	WHERE FA.FolderId = @FolderId 
		AND FA.ProjectId = @ProjectId 
		AND FA.FileId = @FileId
		AND FA.RevisionNumber = @RevisionNumber
		AND FA.FileNameHash =  HASHBYTES('SHA1', LOWER(@FileName)) 

	IF @AttachmentId IS NULL
	BEGIN
		DECLARE @FileAttachmentId bigint
		SET  @FileAttachmentId = 0
		
		SELECT 0, 0, @FileAttachmentId  -- Nothing exists

		INSERT INTO @FilesTable (FileId)
		VALUES(@FileId)
		exec usp_GetFilesInfo @UserName, @FilesTable

	END
	ELSE
	BEGIN
		SELECT (1) as IsFileExists, 
				(CASE WHEN FA.CreatedBy = @UserId THEN 1 ELSE 0 END) As CreatedByCurrentUser,
		  FA.FileAttachmentId
		FROM FileAttachments2 AS FA 
		WHERE FA.FileAttachmentId = @AttachmentId
			
		--if it is created by current user than only return info need to upload the file
		if(@CreatedBy = @UserId)
		BEGIN
			INSERT INTO @FilesTable (FileId)
			VALUES(@FileId)
			exec usp_GetFilesInfo @UserName, @FilesTable
		END
	END

	--SELECT FA.FileId, FA.VersionPath   
	--FROM FileAttachments2 as FA
	--JOIN @FilesTable AS FT on FT.FileId = FA.FileId 
	--	AND FA.RevisionNumber = FT.RevisionNumber
	--	AND 
	----SELECT FA.FileId, FA.RevisionNumber, FA.FileName, FA.VersionPath, FA.IsFromApp, 
	----	FA.Comments, FA.FileExtension, FA.FileSize, FA.ContentType, FA.CreatedDate, u.FullName, u.LastName, u.Email
	----FROM FileAttachments2 as FA
	----JOIN @FilesTable AS FT on FT.FileId = FA.FileId
	----JOIN UserCache as u on FA.CreatedBy = u.UserId

	

RETURN 0

