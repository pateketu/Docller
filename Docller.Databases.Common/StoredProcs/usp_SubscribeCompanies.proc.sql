CREATE PROCEDURE [dbo].[usp_SubscribeCompanies]
	@CustomerId as bigint,
	@ProjectId as int,
	@ProjectPermissionFlag int,
	@AddAllFolders bit,
	@FolderId as int,
	@FolderPermissionFlag as int,
	@Users as UserTableType READONLY,
	@Companies as CompanyTableType READONLY
AS
		SET XACT_ABORT ON

		SELECT /*u.UserId,*/u.UserName, u.CompanyName, /*cu.CompanyId,*/ dbc.CompanyName as ExistingCompany
		FROM @Users as u JOIN @Companies as c
				ON HASHBYTES('SHA1', LOWER(u.CompanyName)) = HASHBYTES('SHA1', LOWER(c.CompanyName))
		LEFT JOIN CompanyUsers as cu on u.UserId = cu.UserId
		JOIN Companies as dbc ON cu.CompanyId = dbc.CompanyId
		WHERE HASHBYTES('SHA1', LOWER(u.CompanyName)) != HASHBYTES('SHA1', LOWER(dbc.CompanyName))

		IF @@ROWCOUNT > 0
		BEGIN
			RETURN 109 -- UserPartOfAnotherCompany
		END		
	
	
	DECLARE @ReturnCode     int      
	DECLARE @TranStarted   bit	

	SET @ReturnCode = 0
	SET @TranStarted = 0	
	
	DECLARE @CompanyIdsTracker AS TABLE(
		[CompanyId] [bigint],
		[CompanyName] [nvarchar](255),
		[IsNew] bit
	)
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE -- This is to avoid race conditions across all read in stored proc
	
	--Start transaction to insert records
	IF( @@TRANCOUNT = 0 )
	BEGIN
		BEGIN TRANSACTION
		SET @TranStarted = 1
	END
	ELSE
	BEGIN
		SET @TranStarted = 0
	END

	BEGIN TRY

			--Figure out which new companies needs to be added	
			INSERT INTO @CompanyIdsTracker (CompanyName, CompanyId,IsNew)
			SELECT c.CompanyName, c.CompanyId,1
			  FROM @Companies as c
				LEFT JOIN Companies as ac 
					on HASHBYTES('SHA1', LOWER(c.CompanyName)) = HASHBYTES('SHA1', LOWER(ac.CompanyName))
			  WHERE ac.CompanyId is NULL

			  --Get the correctIds of any existing companies
			  INSERT INTO @CompanyIdsTracker (CompanyName, CompanyId,IsNew)
			SELECT c.CompanyName, ac.CompanyId,0
			  FROM @Companies as c
				LEFT JOIN Companies as ac 
					on HASHBYTES('SHA1', LOWER(c.CompanyName)) = HASHBYTES('SHA1', LOWER(ac.CompanyName))
			  WHERE ac.CompanyId is NOT NULL
	
	


			--Insert everything into companies table
			INSERT INTO Companies (CustomerId, CompanyId,  CompanyName)
					SELECT @CustomerId,CompanyId, CompanyName
					FROM @CompanyIdsTracker
					WHERE IsNew = 1

			-- NOw Insert Users into UserCache if they are not already there
			exec usp_AddToUserCache @CustomerId, null, @Users
			
			--Figure out CompanyIds for Users
			DECLARE @CompanyUsers as UserTableType


			INSERT INTO @CompanyUsers(UserId,UserName,CompanyId)
			SELECT u.UserId,u.UserName, c.CompanyId
			FROM @Users as u JOIN @CompanyIdsTracker as c
					ON HASHBYTES('SHA1', LOWER(u.CompanyName)) = HASHBYTES('SHA1', LOWER(c.CompanyName))
	


			INSERT INTO CompanyUsers(CustomerId,CompanyId,UserId)
			SELECT @CustomerId, nc.CompanyId,nc.UserId
			FROM @CompanyUsers as nc 
				LEFT JOIN CompanyUsers as cu 
					ON cu.UserId = nc.UserId 
			WHERE cu.UserId IS NULL
			
			
			-- Insert into ProjectUsers table
			exec usp_AddProjectUsers @CustomerId, @ProjectId,@ProjectPermissionFlag,@Users 

			DECLARE @Folders as FolderTableType
			DECLARE @AllCompanies as CompanyTableType

			IF @FolderId > 0
			BEGIN
				
				INSERT INTO @AllCompanies(CompanyId, CompanyName)
				SELECT CompanyId, CompanyName FROM @CompanyIdsTracker

				INSERT INTO @Folders(FolderId)
				VALUES(@FolderId)

				exec usp_AddCompanyFolderPermissions @CustomerId,@FolderPermissionFlag,0,@Folders,@AllCompanies
			END
			ELSE IF @AddAllFolders = 1
			BEGIN
				INSERT INTO @AllCompanies(CompanyId, CompanyName)
				SELECT CompanyId, CompanyName FROM @CompanyIdsTracker

				INSERT INTO @Folders(FolderId)
				SELECT FolderId FROM FOlders where ProjectId = @ProjectId

				exec usp_AddCompanyFolderPermissions @CustomerId,@ProjectPermissionFlag,1,@Folders,@AllCompanies
			END
			
	IF( @TranStarted = 1 )
	BEGIN
		SET @TranStarted = 0
		COMMIT TRANSACTION				
	END		

END TRY
BEGIN CATCH
	IF( @TranStarted = 1 )
	BEGIN
		SET @TranStarted = 0
		ROLLBACK TRANSACTION		
		exec usp_Rethrow
	END
END CATCH
RETURN @ReturnCode

