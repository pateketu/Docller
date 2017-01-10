CREATE PROCEDURE [dbo].[usp_ShareFiles]
	@CustomerId bigint,
	@TransmittalId bigint,
	@ProjectId bigint,	
	@Message nvarchar(2000),
	@CreatedByUserName nvarchar(400),
	@Files as FileTableType READONLY,
	@Users as UserTableType READONLY
AS
	
SET XACT_ABORT ON

	BEGIN TRY		
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


	--- insert into Transmittals table
	INSERT INTO Transmittals(TransmittalId,CustomerId,ProjectId,Message,IsQuickShare,CreatedBy)
	VALUES (@TransmittalId,@CustomerId,@ProjectId,@Message,1, @UserId)

	-- insert into TransmittedFile
	INSERT INTO TransmittedFiles(TransmittalId,CustomerId,FileId,RevisionNumber)
	SELECT @TransmittalId, @CustomerId, TP.FileId, COALESCE(F.CurrentRevision, 1)
	FROM @Files AS TP JOIN Files AS F ON TP.FileId = F.FileId

	
	--insert into QuickFileShareDistribution
	INSERT INTO QuickFileShareDistribution(TransmittalId,CustomerId,Email,UserId)
	SELECT @TransmittalId,@CustomerId, Email, UserId FROM @Users


	
	IF( @TranStarted = 1 )
	BEGIN
		SET @TranStarted = 0
		COMMIT TRANSACTION			

		SELECT F.FileId,F.FileName, f.FileExtension, f.FileInternalName,f.FileSize--, folders.FullPath  
		FROM Files as F 
			JOIN TransmittedFiles as tf on f.FileId = tf.FileId AND tf.TransmittalId = @TransmittalId
			--JOIN Folders as folders on f.FolderId = folders.FolderId

		RETURN 0
	END	
	END TRY
	BEGIN CATCH	
		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION		

			exec usp_Rethrow 			
			RETURN -1
    END CATCH
