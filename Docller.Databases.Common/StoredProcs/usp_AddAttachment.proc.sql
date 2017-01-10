CREATE PROCEDURE [dbo].[usp_AddAttachment]
	@CustomerId bigint,
	@FileId bigint,
	@FileName nvarchar(255), 	
	@FileExtension nvarchar(10),
	@BaseFileName nvarchar(255),
	@ContentType nvarchar(255),
	@VersionPath nvarchar(50) = NULL,
	@FileSize int,
	@ProjectId int,
	@FolderId int,
	@UserName nvarchar(255),	
	@ParentFile uniqueidentifier,
	@AddAsNewVersion bit
AS
	DECLARE @UserId int 
	DECLARE @ExistingFile bit
	 
	exec @UserId = usp_GetTenantUserId @UserName;

	IF @FileId <= 0
	BEGIN
		--Get it from the ParentFile
		SELECT @FileId = FileId FROM Files WHERE FileInternalName = @ParentFile
	END

	IF EXISTS(SELECT FileId FROM FileAttachments 
				 WHERE FileId = @FileId 
				 AND FileNameHash = HASHBYTES('SHA1', LOWER(BaseFileName)))
	BEGIN
		SET @ExistingFile = 1;
	END
	ELSE
	BEGIN
		SET @ExistingFile = 0;
	END

	IF @AddAsNewVersion = 0 AND @ExistingFile =1
	BEGIN
			RETURN 106 -- EXISTING Files
	END
	ELSE IF ((@AddAsNewVersion = 1 AND @ExistingFile =1) AND @VersionPath IS NULL)
	BEGIN
		RETURN 107 -- Version Path Cannot be null
	END
	BEGIN
			
			DECLARE @CurrentVersion int
			SELECT @CurrentVersion = CurrentRevision FROM Files WHERE FileId = @FileId;
			SET XACT_ABORT ON
			DECLARE @TranStarted   bit
			SET @TranStarted = 0

			 
			BEGIN TRY				
				 IF( @@TRANCOUNT = 0 AND  @ExistingFile = 1 )
				 BEGIN
					BEGIN TRANSACTION
					SET @TranStarted = 1
				 END
			
				IF @ExistingFile = 1 
				BEGIN
					UPDATE FileAttachments
					SET VersionPath = @VersionPath
					WHERE FileId = @FileId AND RevisionNumber = (@CurrentVersion - 1)
				END
				IF @@ROWCOUNT = 1 OR @ExistingFile = 0
				BEGIN
					INSERT INTO FileAttachments(FileId,CustomerId,FileName,FileExtension, ContentType, BaseFileName,RevisionNumber,FileSize,ProjectId,FolderId,CreatedBy,ModifiedBy)
					VALUES(@FileId,@CustomerId,@FileName,@FileExtension, @ContentType,@BaseFileName,@CurrentVersion, @FileSize,@ProjectId,@FolderId,@UserId,@UserId)
					IF @TranStarted = 1
						COMMIT TRANSACTION
				END
				ELSE
				BEGIN
					IF @TranStarted = 1
						ROLLBACK TRANSACTION
					return -1;
				END
			END TRY
			BEGIN CATCH
				IF @TranStarted = 1
					ROLLBACK TRAN
					exec usp_Rethrow
					return -1;
			END CATCH

	END
		

RETURN 0