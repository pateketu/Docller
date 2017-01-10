CREATE PROCEDURE [dbo].[usp_GetUserCompanyId]
	@UserName nvarchar(400)
AS
	DECLARE @CompanyId int
	DECLARE @UserId int

	exec @UserId = usp_GetTenantUserId @UserName

	SELECT @CompanyId = CompanyId
	FROM CompanyUsers 
	WHERE UserId = @UserId

	IF @CompanyId = NULL
		SET @CompanyId = 0

	RETURN @CompanyId
RETURN 0