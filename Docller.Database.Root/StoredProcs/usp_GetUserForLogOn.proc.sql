CREATE PROCEDURE [dbo].[usp_GetUserForLogOn]
	@UserName nvarchar(400)
AS
	SELECT UserId, UserName, FirstName,LastName,Email,IsLocked,ForcePasswordChange,[Password], PasswordSalt, CreatedDate, FailedLogInAttempt
	FROM Users
	WHERE UserNameHash = HASHBYTES('SHA1', LOWER(@UserName))
RETURN 0