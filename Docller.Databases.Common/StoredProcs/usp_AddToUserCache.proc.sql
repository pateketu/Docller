CREATE PROCEDURE [dbo].[usp_AddToUserCache]
	 @CustomerId BIGINT,
	 @UserPermissions int = NULL,
	 @UsersTable UserTableType READONLY

AS
	  
	  DECLARE @UniqueUsers UserTableType
	  
	  INSERT INTO @UniqueUsers (UT.UserId,UT.FirstName, UT.LastName, UT.Email, UT.UserName)
					  SELECT UT.UserId,UT.FirstName, UT.LastName, UT.Email, UT.UserName
					  FROM @UsersTable AS UT 
					  WHERE UT.UserId NOT IN (SELECT UserId  FROM UserCache WITH (HOLDLOCK))

	 IF (@UserPermissions IS NULL)
	 BEGIN
		 INSERT INTO UserCache  (UserId, UserName, Email, FirstName, LastName, CustomerId)
			SELECT UserId, UserName, Email, FirstName, LastName, @CustomerId
				FROM @UniqueUsers
	 END
	 ELSE
	 BEGIN
		INSERT INTO UserCache  (UserId, UserName, Email, FirstName, LastName, CustomerId, UserPermissions)
			SELECT UserId, UserName, Email, FirstName, LastName, @CustomerId, @UserPermissions
				FROM @UniqueUsers
	 END
RETURN 0