CREATE PROCEDURE [dbo].[usp_GetTenantUserId]
		@UserName nvarchar(400)
AS
	DECLARE @UserId as int

	SELECT @UserId = UserId
	FROM UserCache 
	WHERE UserNameHash = HASHBYTES('SHA1', LOWER(@UserName))

	IF @UserId IS NULL
		SET @UserId = 0

RETURN @UserId