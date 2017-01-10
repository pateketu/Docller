CREATE PROCEDURE [dbo].[usp_AddAttachment2]
	@CustomerId bigint,
	@FileAttachmentId bigint,
	@ParentFileAttachmentId bigint,
	@FileId bigint,
	@FileName nvarchar(255), 	
	@FileExtension nvarchar(10),
	@ContentType nvarchar(255),
	@VersionPath nvarchar(50) = NULL,
	@FileSize int,
	@ProjectId int,
	@FolderId int,
	@UserName nvarchar(255),	
	@IsFromApp bit,
	@AppDetails nvarchar(255) = null,
	@Comments nvarchar(2000) = null,
	@AddAsNewVersion bit	
AS
	DECLARE @UserId int 
	DECLARE @CurrentFileVersion int
	DECLARE @AttachmentVersionNum int 
	exec @UserId = usp_GetTenantUserId @UserName;
	SELECT @CurrentFileVersion = CurrentRevision FROM Files WHERE FileId = @FileId;
	IF @AddAsNewVersion = 1 AND @VersionPath IS NULL
	BEGIN
		RETURN 107 -- Version Path Cannot be null
	END
	SELECT @AttachmentVersionNum = (MAX(VersionNumber) + 1) 
		FROM FileAttachments2 WHERE FileAttachmentId = @ParentFileAttachmentId

	IF @AttachmentVersionNum IS NULL
		SET @AttachmentVersionNum = 1

	INSERT INTO [dbo].[FileAttachments2]
           ([FileAttachmentId]
		   ,[ParentFileAttachmentId]
           ,[FileId]
           ,[CustomerId]
           ,[VersionPath]
           ,[VersionNumber]
           ,[RevisionNumber]
           ,[FileName]
           ,[FileExtension]
           ,[ContentType]
           ,[Comments]
           ,[FileSize]
           ,[ProjectId]
           ,[FolderId]
           ,[IsFromApp]
           ,[AppDetails]
           ,[CreatedBy]
           ,[ModifiedBy])
	VALUES(@FileAttachmentId, @ParentFileAttachmentId, @FileId, @CustomerId, @VersionPath, @AttachmentVersionNum, @CurrentFileVersion, @FileName,
		  @FileExtension,@ContentType,@Comments, @FileSize, @ProjectId, @FolderId,@IsFromApp,@AppDetails,@UserId, @UserId)


RETURN 0