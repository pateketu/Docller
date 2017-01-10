CREATE PROCEDURE [dbo].[usp_AddCustomer]
	@CustomerName nvarchar(255),
	@DomainUrl nvarchar(1000),
	@AdminEmail nvarchar(400),
	@Password nvarchar(128),
	@PasswordSalt nvarchar(128),
	@IsTrial bit,
	@ImageUrl nvarchar(255)=null,
	@IsExistingUser bit = 0 OUTPUT,
	@NewCustomerId BIGINT = 0 OUTPUT,
	@AdminUserId BIGINT = 0 OUTPUT
AS
	SET XACT_ABORT ON

	DECLARE @TranStarted   bit	

	SET @TranStarted = 0

	-- Check for Dup Customer	
	 IF EXISTS(SELECT CustomerName FROM Customers WHERE HASHBYTES('SHA1', LOWER(@CustomerName)) = CustomerNameHash) 
	 BEGIN
		RETURN 101 -- Duplicate Customer		
	 END

	 DECLARE @DomainExists int
	 exec @DomainExists = usp_IsDomainUrlExists @DomainUrl

	 IF @DomainExists = 1
	 BEGIN
		RETURN 102 -- Duplicate Url		
	 END

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

		-- Insert the Admin User
		DECLARE @AdminUser as UserTableType
		DECLARE @InsertedAdminUser as UserTableType

		INSERT INTO @AdminUser (Email,[Password],PasswordSalt, UserName)
		VALUES(@AdminEmail,  @Password, @PasswordSalt, @AdminEmail)

		DECLARE @UserAddOperationRetrun int

		INSERT INTO @InsertedAdminUser(UserId,UserName)
		exec @UserAddOperationRetrun = usp_AddUsers @AdminUser
	
	
		IF @UserAddOperationRetrun = 103
		BEGIN
			set @IsExistingUser = 1
		END	

		exec @AdminUserId = usp_GetUserId @AdminEmail

		--SELECT @UserId = UserId FROM @InsertedAdminUser

		--DECLARE @CustomerCompanyId int = 0
		-- Add into Companies table
		--exec usp_AddCompany @CustomerName,null,@CompanyId=@CustomerCompanyId output
	         
		--SET @ErrorCode = @@ERROR
		--IF( @ErrorCode <> 0 )
		--   GOTO Cleanup

		-- INSERT INTO Customers table
		INSERT INTO Customers(CustomerName, AdminUserId, IsTrial,ImageUrl, DomainUrl)
		VALUES(@CustomerName, @AdminUserId, @IsTrial,@ImageUrl, @DomainUrl)
	
		SET @NewCustomerId = SCOPE_IDENTITY();

	
		--exec usp_AddCompanyUser @CustomerId, @UserId, @CustomerCompanyId
	
		--SET @ErrorCode = @@ERROR
		--IF( @ErrorCode <> 0 )
		--   GOTO Cleanup
	   		
		IF( @TranStarted = 1 )
		BEGIN
			SET @TranStarted = 0
			COMMIT TRANSACTION
			RETURN 0
		END		

	END TRY
	BEGIN CATCH
		IF @TranStarted = 1
			ROLLBACK TRAN
	
		exec usp_Rethrow
		return -1;
	END CATCH