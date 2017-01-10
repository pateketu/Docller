CREATE PROCEDURE [dbo].[usp_GetCustomer]
	@DomainUrl nvarchar (1000),
	@CustomerId bigint
AS
	IF @DomainUrl IS NULL
	BEGIN
		SELECT CustomerId, CustomerName, ImageUrl, IsTrial, DomainUrl, CreatedDate
		FROM Customers
		WHERE CustomerId = @CustomerId
	END
	ELSE
	BEGIN
		SELECT CustomerId, CustomerName, ImageUrl, IsTrial, DomainUrl, CreatedDate
		FROM Customers
		Where DomainUrlHash = HASHBYTES('SHA1',lower(@DomainUrl))
	END
RETURN 0