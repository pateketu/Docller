CREATE PROCEDURE [dbo].[usp_UpdateUser]
	@UserName nvarchar(400),
	@FirstName nvarchar(255) = null, 
	@LastName nvarchar(255) = null, 
	@Password nvarchar(128),
	@PasswordSalt nvarchar(128)
AS

	IF LEN(@FirstName) > 0 AND LEN(@LastName) > 0
	BEGIN

		Update Users
		SET FirstName = @FirstName,
		LastName = @LastName,
		[Password] = @Password,
		PasswordSalt = @PasswordSalt,
		ForcePasswordChange = 0
		WHERE UserNameHash = HASHBYTES('SHA1', LOWER(@UserName))

	END
	ELSE
	BEGIN
		Update Users
		SET [Password] = @Password,
		PasswordSalt = @PasswordSalt,
		ForcePasswordChange = 1
		WHERE UserNameHash = HASHBYTES('SHA1', LOWER(@UserName))
	END
RETURN 0