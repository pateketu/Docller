CREATE PROCEDURE [dbo].[usp_UpdateFailedLoginAttempt]
	-- Add the parameters for the stored procedure here
	@UserName NVARCHAR(400),
	@FailedLogInAttempt INT,
	@IsLocked BIT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Update Users
	SET FailedLogInAttempt = @FailedLogInAttempt,
		IsLocked=@IsLocked
	WHERE UserNameHash = HASHBYTES('SHA1', LOWER(@UserName))

	RETURN 0
END
GO