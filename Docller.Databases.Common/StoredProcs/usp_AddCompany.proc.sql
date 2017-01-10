CREATE PROCEDURE [dbo].[usp_AddCompany]
	@CustomerId BIGINT,
	@CompanyId bigint,
	@CompanyName nvarchar(255), 
	@CompanyAlias nvarchar(5) = NULL	
AS
	/* To avoid race condition in EXISTS http://weblogs.sqlteam.com/dang/archive/2007/10/28/Conditional-INSERTUPDATE-Race-Condition.aspx */
	IF NOT EXISTS(SELECT CompanyId FROM Companies WITH (HOLDLOCK) WHERE HASHBYTES('SHA1', LOWER(@CompanyName)) = CompanyNameHash)
	BEGIN
		INSERT INTO Companies (CompanyId, CustomerId,  CompanyName, CompanyAlias)
		VALUES(@CompanyId,  @CustomerId, @CompanyName,@CompanyAlias)	
	END
	
	