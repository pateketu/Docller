CREATE TABLE [dbo].[QuickFileShareDistribution]
(
	TransmittalId bigint not null,
	CustomerId bigint not null   CHECK(CustomerId > 0),
	Email nvarchar(400) not null,
	[EmailHash]  AS (CONVERT([varbinary](20),hashbytes('SHA1',lower([Email])),(0))) PERSISTED NOT NULL,
	UserId int null	
	PRIMARY KEY (TransmittalId, EmailHash, CustomerId),
	CONSTRAINT FK_QuickFileShareDistribution_Transmittals FOREIGN KEY(TransmittalId, CustomerId) 
		REFERENCES Transmittals(TransmittalId, CustomerId)
)FEDERATED ON (cid=CustomerId)
