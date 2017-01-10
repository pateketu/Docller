CREATE PROCEDURE [dbo].[usp_GetUserLogonInfo]
	@UserName nvarchar(400),
	@CustomerId bigint
	AS
	DECLARE @UserId int
	DECLARE @IsCustomerAdmin bit
	SET @IsCustomerAdmin = 0
	exec @UserId = usp_GetUserId @UserName
	
	IF EXISTS(SELECT AdminUserId FROM Customers WHERE CustomerId = @CustomerId AND AdminUserId = @UserId)
	BEGIN
		SET @IsCustomerAdmin = 1
	END
	
	SELECT u.FirstName, u.LastName, @IsCustomerAdmin AS IsCustomerAdmin,
	u.FailedLogInAttempt, u.IsLocked
	FROM Users as u 		 
	WHERE u.UserId = @UserId

	

RETURN 0