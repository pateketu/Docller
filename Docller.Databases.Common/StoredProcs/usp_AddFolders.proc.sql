Create PROCEDURE [dbo].[usp_AddFolders]
	@UserName nvarchar(400),
	@CustomerId bigint,
	@ProjectId bigint,
	@ParentFolderId bigint,
	@PermissionMask int,
	@Seperator VARCHAR(1),
	@Folders FolderTableType READONLY	
AS

	DECLARE @UserId int
	DECLARE @CompanyId int
	DECLARE @TranStarted   bit
	DECLARE @ParentPath nvarchar(2000)
	DECLARE @ParentFullPath nvarchar(2000)

	SET XACT_ABORT ON
	--First remove duplicate folders
	IF @ParentFolderId = 0
	BEGIN
		SET @ParentFolderId = null
		SET @ParentPath = ''
	END
	ELSE
	BEGIN
		SELECT @ParentFullPath = FullPath FROM Folders WHERE FolderId = @ParentFolderId
		SET @ParentPath = @ParentFullPath + @Seperator
	END

	DECLARE @DupsFolders FolderTableType

	INSERT INTO @DupsFolders(FolderName)
		SELECT F.FolderName
		FROM Folders as F
			JOIN @Folders AS Temp
			ON F.FolderNameHash = HASHBYTES('SHA1', LOWER(Temp.FolderName))
		WHERE F.ProjectId = @ProjectId AND (F.ParentFolderId = @ParentFolderId OR @ParentFolderId IS NULL)

	
	exec @UserId = usp_GetTenantUserId @UserName
	SELECT @CompanyId  = CompanyId 
	FROM CompanyUsers
	WHERE UserId = @UserId

	SET @TranStarted = 0

	BEGIN TRY
	IF( @@TRANCOUNT = 0 )
	 BEGIN
		BEGIN TRANSACTION
		SET @TranStarted = 1
	 END
	 ELSE
	 BEGIN
		SET @TranStarted = 0
	END

	DECLARE @NewFolders AS FolderTableType
	--first insert into Folders Table
	INSERT INTO Folders(FolderId, CustomerId, ProjectId, FolderName, ParentFolderId)
				OUTPUT INSERTED.FolderId
				INTO @NewFolders (FolderId)
				SELECT nf.FolderId, @CustomerId, @ProjectId, nf.FolderName, @ParentFolderId
				FROM @Folders as nf
				LEFT JOIN @DupsFolders as df ON 
					HASHBYTES('SHA1', LOWER(nf.FolderName)) = HASHBYTES('SHA1', LOWER(df.FolderName))
				WHERE df.FolderName IS NULL					
	
	
	-- update full path
	UPDATE Folders
		SET FullPath = @ParentPath + FolderInternalName 
	WHERE FolderId IN (SELECT FolderId FROM @NewFolders)	

	DECLARE @Companies as CompanyTableType
	INSERT INTO @Companies (CompanyId)
	VALUES(@CompanyId)
	--insert into the permissions
	exec usp_AddCompanyFolderPermissions  @CustomerId, @PermissionMask, 1, @NewFolders,@Companies
	--INSERT INTO CompanyFolderPermissions(FolderId,CompanyId,CustomerId, PermissionMask)
		--		SELECT FolderId, @CompanyId, @CustomerId, @PermissionMask
			--	FROM @NewFolders
	
		
	IF( @TranStarted = 1 )
	BEGIN
		SET @TranStarted = 0
		COMMIT TRANSACTION
		--See of we need to return s status of 105 indicating
		--duplicate folders
		IF EXISTS(SELECT FolderName FROM @DupsFolders)
		BEGIN
			-- return the dup folder names so the app
			--can show approrpiate message
			SELECT FolderName FROM @DupsFolders
			RETURN 105
		END
		RETURN 0
	END		
	END TRY
	BEGIN CATCH
		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION
		exec usp_Rethrow
	END CATCH
	