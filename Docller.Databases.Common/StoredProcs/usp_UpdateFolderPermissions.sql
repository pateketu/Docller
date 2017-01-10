CREATE PROCEDURE [dbo].[usp_UpdateFolderPermissions]
	@ProjectId bigint,
	@FolderId bigint,
	@Companies as CompanyTableType READONLY
AS

SET XACT_ABORT ON

	DECLARE @TranStarted   bit	
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
	
		UPDATE cfp
		Set PermissionMask = c.PermissionFlag
		FROM CompanyFolderPermissions as cfp
			JOIN @Companies as c on cfp.CompanyId = c.CompanyId and c.PermissionFlag IS NOT NULL
		WHERE cfp.FolderId = @FolderId

		DELETE cfp
		FROM CompanyFolderPermissions as cfp
			JOIN @Companies as c on cfp.CompanyId = c.CompanyId and c.PermissionFlag IS NULL
		WHERE cfp.FolderId = @FolderId

		IF( @TranStarted = 1 )
		BEGIN
			SET @TranStarted = 0
			COMMIT TRANSACTION
			RETURN 0
		END	

	END TRY
	BEGIN CATCH	
		IF( @TranStarted = 1 )
			ROLLBACK TRANSACTION		

			exec usp_Rethrow 
    END CATCH