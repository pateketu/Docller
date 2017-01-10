CREATE PROCEDURE [dbo].[usp_GetFilePrefs]
	@UserName nvarchar(400)
AS
	DECLARE @CompanyId int

	exec @CompanyId = usp_GetUserCompanyId @UserName

	SELECT c.RevisionRegEx,c.AttributeMappingsXml
	FROM Companies as c
	WHERE C.CompanyId = @CompanyId

RETURN 0