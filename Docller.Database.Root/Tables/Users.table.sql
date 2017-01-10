CREATE TABLE [Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](400) NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[PasswordSalt] [nvarchar](128) NOT NULL,
	[FirstName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NULL,
	[Email] [nvarchar](400) NOT NULL,
	[UserNameHash]  AS (CONVERT([varbinary](20),hashbytes('SHA1',lower([UserName])),(0))) PERSISTED NOT NULL,
	[IsLocked] [bit] NOT NULL default(0),
	[FailedLogInAttempt] int NOT NULL default(0),
	[ForcePasswordChange] [bit] NOT NULL default(1),
	[CreatedDate] datetime default(getdate()) NOT NULL 
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)
)