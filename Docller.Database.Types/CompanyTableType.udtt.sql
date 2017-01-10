CREATE TYPE [dbo].[CompanyTableType] AS TABLE 
(
	[CompanyId] [bigint] NULL,
	[CompanyName] [nvarchar](255) NULL,
	[PermissionFlag] int NULL
)