CREATE TABLE [Customers](
	[CustomerId] [bigint] IDENTITY(1,1) NOT NULL,
	[AdminUserId] int NOT NULL,
	[CustomerName] [nvarchar](255) NOT NULL,
	[IsTrial] bit NOT NULL,	
	[ImageUrl] [nvarchar] (255) NULL,	
	[CustomerNameHash]  AS (CONVERT([varbinary](20),hashbytes('SHA1',lower([CustomerName])),(0))) PERSISTED NOT NULL,
	[DomainUrl] [nvarchar] (1000) NOT NULL,
	[DomainUrlHash]  AS (CONVERT([varbinary](20),hashbytes('SHA1',lower([DomainUrl])),(0))) PERSISTED NOT NULL,
	[CreatedDate] datetime default(getdate()) NOT NULL	
CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)

CONSTRAINT FK_Customers_AdminUser FOREIGN KEY(AdminUserId) 
		 REFERENCES Users(UserId)
)
