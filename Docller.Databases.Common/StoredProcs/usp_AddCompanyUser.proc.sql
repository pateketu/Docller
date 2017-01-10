CREATE PROCEDURE [dbo].[usp_AddCompanyUser]
	@CustomerId bigint,
	@UserId int,
	@CompanyId int
AS
	
	--DECLARE @CompanyName nvarchar(255)

	--SELECT @CompanyName = CompanyName FROM Companies WHERE CompanyId = @CompanyId

	INSERT INTO CompanyUsers (CustomerId, UserId, CompanyId)
	VALUES(@CustomerId, @UserId, @CompanyId);

RETURN 0