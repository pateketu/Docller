CREATE TABLE [dbo].[Projects](
	[ProjectId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL  CHECK(CustomerId > 0),
	[ProjectName] [nvarchar](255) NOT NULL,
	[ProjectNameHash]  AS (CONVERT([varbinary](20),hashbytes('SHA1',lower([ProjectName])),(0))) PERSISTED NOT NULL,
	[ProjectCode] [nvarchar](10)  NULL,
	[BlobContainer] [nvarchar](255) NULL,	
	[ProjectImage] [nvarchar](255) NULL,
	[CreatedDate] datetime default(getdate()) NOT NULL 
	PRIMARY KEY (ProjectId, CustomerId) 
)FEDERATED ON (cid=CustomerId)