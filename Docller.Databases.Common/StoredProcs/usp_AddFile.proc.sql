CREATE PROCEDURE [dbo].[usp_AddFile]
	@CustomerId bigint,
	@FileId bigint,
	@FileInternalName uniqueidentifier,
	@FileName nvarchar(255), 	
	@ContentType nvarchar(255),
	@FileExtension nvarchar(10),
	@BaseFileName nvarchar(255),
	@DocNumber nvarchar(255),
	@VersionPath nvarchar(50) = NULL,
	@FileSize int,
	@ProjectId int,
	@FolderId int,
	@UserName nvarchar(255),	
	@AddAsNewVersion bit
AS
	DECLARE @UserId int 
	DECLARE @ExistingFile bit

	exec @UserId = usp_GetTenantUserId @UserName;

	IF EXISTS(SELECT FileId FROM Files WHERE FileId = @FileId)
	BEGIN
		SET @ExistingFile = 1;
	END

	IF @AddAsNewVersion = 1 AND @ExistingFile =1
	BEGIN
			-- copy the version
			INSERT INTO FileVersions(FileId,RevisionNumber,CustomerId,VersionPath,FileName,FileExtension,ContentType,  FileSize,DocNumber,Revision,Title,Notes,ProjectId,FolderId,CreatedBy,ModifiedBy, StatusId)
									SELECT FileId,CurrentRevision,CustomerId,@VersionPath,FileName, FileExtension,ContentType,FileSize,DocNumber,Revision,Title,Notes,ProjectId,FolderId,@UserId,@UserId, StatusId
									FROM Files WHERE FileId = @FileId

			UPDATE Files
			SET FileName = @FileName,
			BaseFileName = @BaseFileName,
			CurrentRevision = CurrentRevision+1,
			FileSize = @FileSize,
			StatusId = NULL,
			Revision = NULL
			WHERE FileId = @FileId
	END
	ELSE IF @AddAsNewVersion = 0 AND  @ExistingFile = 1
	BEGIN
			RETURN 106 -- EXISTING Files
	END
	ELSE
	BEGIN
			INSERT INTO Files(FileId,CustomerId,FileInternalName, FileName, FileExtension,  ContentType, BaseFileName,DocNumber, FileSize,ProjectId,FolderId,CreatedBy,ModifiedBy)
			VALUES(@FileId,@CustomerId, @FileInternalName, @FileName, @FileExtension,  @ContentType, @BaseFileName,@DocNumber, @FileSize,@ProjectId,@FolderId,@UserId,@UserId)
	END
	

	

RETURN 0