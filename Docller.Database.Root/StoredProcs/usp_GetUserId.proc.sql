CREATE PROCEDURE [dbo].[usp_GetUserId]
	@UserName nvarchar(400)
AS
	DECLARE @UserId as int

	SELECT @UserId = UserId
	FROM Users 
	WHERE UserNameHash = HASHBYTES('SHA1', LOWER(@UserName))

	IF @UserId IS NULL
		SET @UserId = 0

RETURN @UserId