CREATE TABLE CompanyUsers
(
	CompanyId BIGINT NOT NULL,
	CustomerId BIGINT NOT NULL CHECK(CustomerId > 0),
	UserId INT NOT NULL,
	--	CompanyName NVARCHAR(255) NOT NULL,
	[CreatedDate] datetime default(getdate()) NOT NULL 
	PRIMARY KEY (CompanyId, CustomerId, UserId),
	CONSTRAINT FK_Companies_CompanyUsers FOREIGN KEY(CompanyId, CustomerId) 
		 REFERENCES Companies(CompanyId, CustomerId)
)FEDERATED ON (cid=CustomerId)