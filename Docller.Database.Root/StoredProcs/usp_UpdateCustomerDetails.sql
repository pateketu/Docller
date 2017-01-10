CREATE PROCEDURE [dbo].[usp_UpdateCustomerDetails]
	@CustomerId bigint,
	@ImageUrl nvarchar(255)
AS
	UPDATE Customers
	set ImageUrl = @ImageUrl
	WHERE CustomerId = @CustomerId
RETURN 0
