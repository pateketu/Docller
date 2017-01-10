CREATE PROCEDURE [dbo].[usp_GetCompanyFolderPersmission]
	@CompanyName nvarchar(255),
	@FolderId bigint	
AS
	SELECT cfp.PermissionMask FROM CompanyFolderPermissions as cfp 
		JOIN Companies as c on cfp.CompanyId = c.CompanyId
		AND c.CompanyNameHash = HASHBYTES('SHA1', LOWER(@CompanyName))
		WHERE FolderId = @FolderId

RETURN 0
