CREATE PROCEDURE [dbo].[usp_UpdateCompanyAndUserForNewCustomer]
	@CustomerId BIGINT,
	@CompanyId bigint,
	@UserId int,
	@CustomerName nvarchar(255), 
	@AdminEmail nvarchar(400),
	@UserPermissions int
AS
	SET XACT_ABORT ON
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE -- This is to avoid race conditions across all read in stored proc
	
	BEGIN TRY
		DECLARE @TranStarted   bit
	
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

		-- Add the Company
		exec usp_AddCompany @CustomerId, @CompanyId, @CustomerName,null
	
			-- Add to UserCache 
		DECLARE @AdminUser as UserTableType
		DECLARE @InsertedAdminUser as UserTableType

		INSERT INTO @AdminUser (UserId, UserName, Email)
		VALUES(@UserId,@AdminEmail, @AdminEmail)

		exec usp_AddToUserCache @CustomerId, @UserPermissions, @AdminUser
	
		-- Add to company users table
		exec usp_AddCompanyUser @CustomerId, @UserId, @CompanyId
	
	
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

	
RETURN 0