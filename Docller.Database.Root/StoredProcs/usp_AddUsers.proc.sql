CREATE PROCEDURE [dbo].[usp_AddUsers]
 (
       @UsersTable UserTableType READONLY
 )
 AS
 BEGIN
 
	  DECLARE @InsertedUsers AS TABLE
	  (
		 UserId INT,
		 UserName nvarchar(400)
	  )
	 
	  DECLARE @DupUsers UserTableType
	  
	  INSERT INTO @DupUsers (UserId, UserName)
					  SELECT U.UserId, U.UserName
					  FROM @UsersTable AS UT JOIN Users  AS U WITH (HOLDLOCK)
							ON HASHBYTES('SHA1', LOWER(UT.UserName)) = U.UserNameHash
	  
	  IF EXISTS(SELECT [UserName] FROM @DupUsers)
	  BEGIN
			SELECT UserId, UserName FROM @DupUsers	
			RETURN 103	
	  END
	  ELSE
	  BEGIN
		 
			INSERT INTO Users  (UserName, [Password], PasswordSalt, Email, FirstName, LastName)
				OUTPUT INSERTED.UserId, INSERTED.UserName
				INTO @InsertedUsers
					SELECT UserName, [Password], PasswordSalt,  Email, FirstName, LastName
				FROM @UsersTable
			
			SELECT UserId, UserName	
			FROM @InsertedUsers
         
	  END   
    RETURN 0

 END