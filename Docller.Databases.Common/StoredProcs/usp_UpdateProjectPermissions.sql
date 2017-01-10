CREATE PROCEDURE [dbo].[usp_UpdateProjectPermissions]
	@ProjectId bigint,
	@Users as UserTableType READONLY
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
	
		UPDATE pu
		Set PermissionMask = u.PermissionFlag
		FROM ProjectUsers as pu 
			JOIN @Users as u on pu.UserId = u.UserId and u.PermissionFlag IS NOT NULL
		WHERE pu.ProjectId = @ProjectId

		DELETE pu
		FROM ProjectUsers as pu 
			JOIN @Users as u on pu.UserId = u.UserId and u.PermissionFlag IS NULL
		WHERE pu.ProjectId = @ProjectId


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