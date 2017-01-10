CREATE TABLE [dbo].[UserCache]
(
	UserId INT NOT NULL, 
	CustomerId BIGINT NOT NULL CHECK(CustomerId > 0),
	[UserName] [nvarchar](400) NOT NULL,
	[UserNameHash]  AS (CONVERT([varbinary](20),hashbytes('SHA1',lower([UserName])),(0))) PERSISTED NOT NULL,
	[FirstName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NULL,
	[Email] [nvarchar](400) NOT NULL,
	[UserPermissions] int NOT NULL default(1)
	PRIMARY KEY (UserId, CustomerId)
	
)FEDERATED ON (cid=CustomerId)