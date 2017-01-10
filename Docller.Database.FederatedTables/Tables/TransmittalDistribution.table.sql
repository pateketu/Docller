CREATE TABLE [dbo].[TransmittalDistribution]
(
	TransmittalId bigint not null,
	CustomerId bigint not null   CHECK(CustomerId > 0),
	UserId int not null,
	Cced bit not null default(0)
	PRIMARY KEY (TransmittalId, UserId, CustomerId),
	CONSTRAINT FK_TransmittalDistribution_UserCache FOREIGN KEY(UserId, CustomerId) 
		 REFERENCES UserCache(UserId, CustomerId),
	CONSTRAINT FK_TransmittalDistribution_Transmittals FOREIGN KEY(TransmittalId, CustomerId) 
		REFERENCES Transmittals(TransmittalId, CustomerId)
)FEDERATED ON (cid=CustomerId)
