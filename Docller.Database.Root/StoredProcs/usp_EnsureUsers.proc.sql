CREATE PROCEDURE [dbo].[usp_EnsureUsers]
 (
       @UsersTable UserTableType READONLY
 )
 AS
 BEGIN
 
	  DECLARE @AllUsers AS TABLE
	  (
		 UserId INT,
		 UserName nvarchar(400),	
		 FirstName nvarchar(255),
		 LastName nvarchar(255),
		 Email nvarchar(400),	 
		 IsNew bit default(1)
	  )
	 SET XACT_ABORT ON
	 DECLARE @TranStarted   bit	

	  SET @TranStarted = 0	
	   BEGIN TRY	 
	  -- Insert any existing users into AllUsers table
	  INSERT INTO @AllUsers (UserId, UserName,IsNew)
					  SELECT U.UserId, U.UserName,0
					  FROM @UsersTable AS UT JOIN Users AS U WITH(HOLDLOCK)
							ON HASHBYTES('SHA1', LOWER(UT.UserName)) = U.UserNameHash
	  
	  --now fiugure out which users need to be added
	  DECLARE @UsersToAdd as UserTableType

	  INSERT INTO @UsersToAdd(UserName, [Password], PasswordSalt, Email)
	  SELECT U.UserName, u.Password, u.PasswordSalt, u.Email
	  FROM @UsersTable as u
		LEFT JOIN @AllUsers as au 
			on HASHBYTES('SHA1', LOWER(u.UserName)) = HASHBYTES('SHA1', LOWER(au.UserName))
	  WHERE au.UserId is NULL

	  --Add any new users
	  INSERT INTO @AllUsers(UserId,UserName)
	  exec usp_AddUsers @UsersToAdd
	  
	  select u.UserId, u.UserName, u.Email, u.FirstName,u.LastName, au.IsNew
	  FROM @AllUsers as au	JOIN Users as u ON au.UserId = u.UserId
	  
	  IF( @TranStarted = 1 )
		BEGIN
			SET @TranStarted = 0
			COMMIT TRANSACTION
			RETURN 0
		END		
    END TRY
	BEGIN CATCH
		IF @TranStarted = 1
			ROLLBACK TRAN
	
		exec usp_Rethrow
		return -1;
	END CATCH

 END