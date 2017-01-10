CREATE TABLE [dbo].[Status]
(
	StatusId bigint NOT NULL,
	[CustomerId] BIGINT NOT NULL CHECK(CustomerId > 0), 
	ProjectId bigint NOT NULL,
	StatusText nvarchar(200) NOT NULL,
	StatusLongText nvarchar(255) NULL
	PRIMARY KEY (StatusId, CustomerId)
	CONSTRAINT FK_Status_Projects FOREIGN KEY(ProjectId, CustomerId)
		 REFERENCES Projects(ProjectId, CustomerId),
)FEDERATED ON (cid=CustomerId)
