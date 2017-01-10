CREATE TABLE ProjectUsers
(
	CustomerId [bigint] NOT NULL  CHECK(CustomerId > 0),
	ProjectId Bigint NOT NULL,
	UserId INT  NOT NULL,
	PermissionMask INT NOT NULL default(0),
	[CreatedDate] datetime default(getdate()) NOT NULL 
	PRIMARY KEY (CustomerId, ProjectId,UserId),
	CONSTRAINT FK_Projects_ProjectUsers FOREIGN KEY(ProjectId, CustomerId) 
		 REFERENCES Projects(ProjectId, CustomerId)
)FEDERATED ON (cid=CustomerId)