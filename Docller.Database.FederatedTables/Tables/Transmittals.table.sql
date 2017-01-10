CREATE TABLE [dbo].[Transmittals]
(
	TransmittalId  bigint NOT NULL,
	CustomerId bigint not null CHECK(CustomerId > 0),
	ProjectId bigint not null,
	TransmittalNumber nvarchar(50) NULL, 
	Subject nvarchar(255) null,
	Message nvarchar(2000) null,
	IsDraft bit default(0),
	IsQuickShare bit default(0),
	StatusId bigint NULL,
	CreatedDate datetime default(getutcdate()) not null,
	[CreatedBy] [int] NOT NULL	
	PRIMARY KEY (TransmittalId, CustomerId),
	CONSTRAINT FK_Transmittal_Projects FOREIGN KEY(ProjectId, CustomerId) 
		 REFERENCES Projects(ProjectId, CustomerId),
	CONSTRAINT FK_Transmittal_Status FOREIGN KEY(StatusId, CustomerId) 
		REFERENCES Status(StatusId, CustomerId),
	CONSTRAINT FK_Transmittal_CreatedBy FOREIGN KEY(CreatedBy, CustomerId) 
		REFERENCES UserCache(UserId, CustomerId),
)FEDERATED ON (cid=CustomerId)
