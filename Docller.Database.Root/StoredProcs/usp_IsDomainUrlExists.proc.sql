CREATE PROCEDURE [dbo].[usp_IsDomainUrlExists]
	@DomainUrl nvarchar(1000)
AS
BEGIN
	declare @IsExists as bit
	if EXISTS (SELECT DomainUrl 
				FROM Customers  
				WHERE HASHBYTES('SHA1', LOWER(@DomainUrl)) = DomainUrlHash)
	BEGIN
		set @IsExists = 1		
	END
	ELSE
	BEGIN
		set @IsExists = 0		
	END
	SELECT @IsExists
	return @IsExists
END