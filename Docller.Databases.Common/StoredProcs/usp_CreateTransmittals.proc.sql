CREATE PROCEDURE [dbo].[usp_CreateTransmittals]
	@CustomerId bigint,
	@TransmittalId bigint,
	@ProjectId bigint,
	@TransmittalNumber nvarchar(50),
	@Subject nvarchar(255),
	@Message nvarchar(2000),
	@IsDraft bit,
	@StatusId bigint,
	@CreatedByUserName nvarchar(400),
	@PermissionsMask int,
	@Files as FileTableType READONLY,
	@Users as UserTableType READONLY
AS
	
	SET XACT_ABORT ON

	BEGIN TRY
	IF @StatusId = 0
	BEGIN
		IF @IsDraft = 1
		BEGIN
			SET @StatusId = NULL
		END
		ELSE
		BEGIN
			-- StratusId is required
			RETURN 108			
		END
	END
		
	DECLARE @TranStarted   bit	

	SET @TranStarted = 0

	DECLARE @UserId as int
	exec @UserId = usp_GetTenantUserId @CreatedByUserName

	
	 IF( @@TRANCOUNT = 0 )
	 BEGIN
		BEGIN TRANSACTION
		SET @TranStarted = 1
	 END
	 ELSE
	 BEGIN
		SET @TranStarted = 0
	 END


	 -- Figure out if it is an existing transmittal
	 IF EXISTS(SELECT TransmittalId FROM Transmittals WHERE TransmittalId = @TransmittalId)
	 BEGIN
			--Update an exisitng one
			UPDATE Transmittals
			SET TransmittalNumber = @TransmittalNumber,
				Subject = @Subject,
				Message = @Message,
				IsDraft = @IsDraft,
				StatusId = @StatusId
			WHERE TransmittalId = @TransmittalId

			-- Delete existing distribution so new one can be just picked up
			DELETE TransmittalDistribution
			WHERE TransmittalId = @TransmittalId

			-- New files cannot be added to saved transmittal but it can be removed, so we need to just make sure user have 
			-- not removed any files

			DECLARE @FilesToRemove AS FileTableType 

			
			INSERT INTO @FilesToRemove (FileId)
			SELECT TF.FileId
			FROM TransmittedFiles AS TF LEFT JOIN @Files AS TP ON TF.FileId = TP.FileId
			WHERE TP.FileId IS NULL

			DELETE TransmittedFiles
			FROM TransmittedFiles AS TF
			JOIN @FilesToRemove AS FTR ON TF.FileId = FTR.FileId
			WHERE TF.TransmittalId = @TransmittalId

	 END
	 ELSE
     BEGIN
		
			--- insert into Transmittals table
			INSERT INTO Transmittals(TransmittalId,CustomerId,ProjectId,TransmittalNumber,Subject,Message,IsDraft,StatusId,CreatedBy)
			VALUES (@TransmittalId,@CustomerId,@ProjectId,@TransmittalNumber,@Subject,@Message,@IsDraft, @StatusId, @UserId)




		-- insert into TransmittedFile
		INSERT INTO TransmittedFiles(TransmittalId,CustomerId,FileId,RevisionNumber)
		SELECT @TransmittalId, @CustomerId, TP.FileId, COALESCE(F.CurrentRevision, 1)
		FROM @Files AS TP JOIN Files AS F ON TP.FileId = F.FileId

	END

	--insert into TransmittalDistribution
	INSERT INTO TransmittalDistribution(TransmittalId,CustomerId,UserId,Cced)
	SELECT @TransmittalId,@CustomerId, UserId, Cced FROM @Users


	--- if this was not a Draft transmittal then update the File status to match transmittal status
	IF @IsDraft = 0
	BEGIN
		UPDATE Files
		SET StatusId = @StatusId
		FROM Files as F 
			JOIN TransmittedFiles AS TF 
				ON f.FileId = TF.FileId AND TF.TransmittalId = @TransmittalId

		-- update folder permissions so user can see the folder

		DECLARE @Folders as FolderTableType
		DECLARE @Companies as CompanyTableType

		INSERT INTO @Folders(FolderId)
		SELECT DISTINCT F.FolderId FROM Files as F 
			JOIN TransmittedFiles AS TF 
				ON f.FileId = TF.FileId AND TF.TransmittalId = @TransmittalId 

		INSERT INTO @Companies (CompanyId)
		SELECT DISTINCT CU.CompanyId 
		FROM CompanyUsers as CU JOIN TransmittalDistribution as TD 
			ON CU.UserId = TD.UserId AND TD.TransmittalId = @TransmittalId
			

		exec usp_AddCompanyFolderPermissions @CustomerId,@PermissionsMask, 0,@Folders,@Companies

	END
	
		IF( @TranStarted = 1 )
		BEGIN
			SET @TranStarted = 0
			COMMIT TRANSACTION
			IF @IsDraft = 0
			BEGIN
				exec usp_GetTransmittalLite @ProjectId, @TransmittalId
				exec usp_GetIssueSheet @CreatedByUserName, @TransmittalId
			END
			RETURN 0
		END	
	END TRY
	BEGIN CATCH	
		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION		

			exec usp_Rethrow 
    END CATCH

RETURN 0