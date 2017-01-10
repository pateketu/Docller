CREATE TYPE [dbo].[UserTableType] AS TABLE(
	[UserId] [int] NULL DEFAULT ((0)),
	[UserName] [nvarchar](400) NULL,
	[FirstName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NULL,
	[Email] [nvarchar](400) NULL,
	[Password] [nvarchar](128) NULL,
	[PasswordSalt] [nvarchar](128) NULL,
	[CompanyId] [bigint] NULL,
	[CompanyName] nvarchar(255),
	[CCed] [bit] NULL,
	[PermissionFlag] int NULL
)