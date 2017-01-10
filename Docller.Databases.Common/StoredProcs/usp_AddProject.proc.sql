CREATE PROCEDURE [dbo].[usp_AddProject]
	@ProjectId bigint,
	@ProjectName nvarchar(255),
	@ProjectCode nvarchar(10),
	@BlobContainer nvarchar(255),
	@ProjectImage nvarchar(255) = null,
	@CustomerId bigint,
	@UserName nvarchar(400),
	@PermissionFlag int,
	@ProjectStatus as StringTableType READONLY
AS
	DECLARE @UserId int
	DECLARE @ErrorCode     int      
	DECLARE @TranStarted   bit
	
	IF EXISTS(SELECT ProjectName FROM Projects
			   WHERE ProjectNameHash = HASHBYTES('SHA1', LOWER(@ProjectName)))
	BEGIN
		RETURN 104 -- Existing Project
	END
	exec @UserId = usp_GetTenantUserId @UserName

	SET @ErrorCode = 0
	SET @TranStarted = 0

	IF( @@TRANCOUNT = 0 )
	 BEGIN
		BEGIN TRANSACTION
		SET @TranStarted = 1
	 END
	 ELSE
	 BEGIN
		SET @TranStarted = 0
	END

	INSERT INTO Projects(ProjectId, ProjectName, ProjectCode, ProjectImage, CustomerId, BlobContainer)
	VALUES(@ProjectId, @ProjectName, @ProjectCode, @ProjectImage, @CustomerId, @BlobContainer)

	SET @ErrorCode = @@ERROR
	IF( @ErrorCode <> 0 )
	   GOTO Cleanup
	
	-- Add ProjectUsers
	DECLARE @UsersToInsert as UserTableType
	INSERT INTO @UsersToInsert (UserId)
	VALUES(@UserId)

	exec usp_AddProjectUsers @CustomerId, @ProjectId, @PermissionFlag,  @UsersToInsert

	SET @ErrorCode = @@ERROR
	IF( @ErrorCode <> 0 )
	   GOTO Cleanup

    exec usp_AddDefaultProjectStatus @CustomerId, @ProjectId,@ProjectStatus
	
	SET @ErrorCode = @@ERROR
	IF( @ErrorCode <> 0 )
	   GOTO Cleanup

	IF( @TranStarted = 1 )
	BEGIN
		SET @TranStarted = 0
		COMMIT TRANSACTION
		RETURN 0
	END		

	Cleanup:
    IF( @TranStarted = 1 )
    BEGIN
        SET @TranStarted = 0
        ROLLBACK TRANSACTION		
    END
	RETURN @ErrorCode

RETURN 0