CREATE TABLE [dbo].[Companies](
	[CompanyId] [bigint] NOT NULL,
	[CustomerId] BIGINT NOT NULL CHECK(CustomerId > 0),
	[CompanyName] [nvarchar](255) NOT NULL,
	[CompanyAlias] [nvarchar](5) NULL,
	[CompanyNameHash]  AS (CONVERT([varbinary](20),hashbytes('SHA1',lower([CompanyName])),(0))) PERSISTED NOT NULL,
	[RevisionRegEx] [nvarchar](50) NULL,
	[AttributeMappingsXml] [nvarchar](3000) NULL,
	[CreatedDate] datetime default(getdate()) NOT NULL	
	PRIMARY KEY (CompanyId, CustomerId) 
)FEDERATED ON (cid=CustomerId)