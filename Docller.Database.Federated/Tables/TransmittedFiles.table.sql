CREATE TABLE [dbo].[TransmittedFiles]
(
	TransmittalId bigint not null,
	CustomerId bigint not null   CHECK(CustomerId > 0),
	FileId bigint not null,
	RevisionNumber int not null
	PRIMARY KEY (TransmittalId, FileId, CustomerId, RevisionNumber),
	CONSTRAINT FK_TransmittedFiles_Files FOREIGN KEY(FileId, CustomerId) 
		 REFERENCES Files(FileId, CustomerId),
	CONSTRAINT FK_TransmittedFiles_Transmittals FOREIGN KEY(TransmittalId, CustomerId) 
		REFERENCES Transmittals(TransmittalId, CustomerId)
)FEDERATED ON (cid=CustomerId)
