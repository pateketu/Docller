CREATE PROCEDURE [dbo].[usp_UpdateUserCache]
	@FirstName nvarchar(255) = null, 
	@LastName nvarchar(255) = null ,
	@UserName nvarchar(400)
AS
	UPDATE UserCache
	SET FirstName = @FirstName,
	LastName = @LastName
	WHERE UserNameHash = HASHBYTES('SHA1', LOWER(@UserName))

RETURN 0